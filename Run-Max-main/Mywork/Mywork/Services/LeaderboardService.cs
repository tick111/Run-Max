using System.Collections.Generic;
using System.Threading;
using Mywork.Models;

namespace Mywork.Services;

public sealed class LeaderboardService : ILeaderboardService
{
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);
    private Node? _root;
    private readonly Dictionary<long, decimal> _scores = new();

    private sealed class Node
    {
        public long CustomerId;
        public decimal Score;
        public Node? Left;
        public Node? Right;
        public Node? Parent;
        public int Height = 1;
        public int Count = 1;
    }

    public decimal UpdateScore(long customerId, decimal delta)
    {
        if (delta is < -1000m or > 1000m)
            throw new ArgumentOutOfRangeException(nameof(delta), "delta must be in [-1000,1000].");

        _lock.EnterWriteLock();
        try
        {
            _scores.TryGetValue(customerId, out var oldScore);
            var newScore = oldScore + delta;
            _scores[customerId] = newScore;

            if (oldScore > 0)
                _root = Remove(_root, oldScore, customerId);

            if (newScore > 0)
                _root = Insert(_root, newScore, customerId, null);

            return newScore;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public IEnumerable<LeaderboardEntryDto> GetRange(int startRank, int endRank)
    {
        if (startRank < 1 || endRank < startRank)
            throw new ArgumentException("Invalid rank range.");

        _lock.EnterReadLock();
        try
        {
            if (_root is null) yield break;

            int total = _root.Count;
            if (startRank > total) yield break;
            if (endRank > total) endRank = total;

            var node = SelectByRank(_root, startRank);
            int currentRank = startRank;
            while (node != null && currentRank <= endRank)
            {
                yield return new LeaderboardEntryDto(node.CustomerId, node.Score, currentRank);
                node = Successor(node);
                currentRank++;
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public IEnumerable<LeaderboardEntryDto>? GetWithNeighbors(long customerId, int high, int low)
    {
        if (high < 0 || low < 0) throw new ArgumentOutOfRangeException();

        _lock.EnterReadLock();
        try
        {
            if (!_scores.TryGetValue(customerId, out var score) || score <= 0)
                return null;

            int rank = GetRank(customerId, score);
            if (rank <= 0) return null;

            int startRank = Math.Max(1, rank - high);
            int endRank = rank + low;
            int total = _root?.Count ?? 0;
            if (endRank > total) endRank = total;

            var list = new List<LeaderboardEntryDto>(endRank - startRank + 1);
            var node = SelectByRank(_root!, startRank);
            int current = startRank;
            while (node != null && current <= endRank)
            {
                list.Add(new LeaderboardEntryDto(node.CustomerId, node.Score, current));
                node = Successor(node);
                current++;
            }
            return list;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    // ---- AVL / Order statistic ----
    private static int Height(Node? n) => n?.Height ?? 0;
    private static int Count(Node? n) => n?.Count ?? 0;
    private static void Recalc(Node n)
    {
        n.Height = Math.Max(Height(n.Left), Height(n.Right)) + 1;
        n.Count = Count(n.Left) + Count(n.Right) + 1;
    }
    private static int BalanceFactor(Node n) => Height(n.Left) - Height(n.Right);

    private Node RotateRight(Node y)
    {
        var x = y.Left!;
        var t2 = x.Right;
        x.Right = y;
        x.Parent = y.Parent;
        y.Parent = x;
        y.Left = t2;
        if (t2 != null) t2.Parent = y;
        Recalc(y); Recalc(x);
        return x;
    }
    private Node RotateLeft(Node x)
    {
        var y = x.Right!;
        var t2 = y.Left;
        y.Left = x;
        y.Parent = x.Parent;
        x.Parent = y;
        x.Right = t2;
        if (t2 != null) t2.Parent = x;
        Recalc(x); Recalc(y);
        return y;
    }
    private Node Balance(Node n)
    {
        Recalc(n);
        int bf = BalanceFactor(n);
        if (bf > 1)
        {
            if (BalanceFactor(n.Left!) < 0)
                n.Left = RotateLeft(n.Left!);
            return RotateRight(n);
        }
        if (bf < -1)
        {
            if (BalanceFactor(n.Right!) > 0)
                n.Right = RotateRight(n.Right!);
            return RotateLeft(n);
        }
        return n;
    }
    private int Compare(decimal scoreA, long idA, decimal scoreB, long idB)
    {
        if (scoreA != scoreB) return scoreA > scoreB ? -1 : 1;
        if (idA != idB) return idA < idB ? -1 : 1;
        return 0;
    }
    private Node Insert(Node? node, decimal score, long customerId, Node? parent)
    {
        if (node == null)
        {
            return new Node { Score = score, CustomerId = customerId, Parent = parent };
        }
        int cmp = Compare(score, customerId, node.Score, node.CustomerId);
        if (cmp < 0) node.Left = Insert(node.Left, score, customerId, node);
        else if (cmp > 0) node.Right = Insert(node.Right, score, customerId, node);
        return Balance(node);
    }
    private Node? Remove(Node? node, decimal score, long customerId)
    {
        if (node == null) return null;
        int cmp = Compare(score, customerId, node.Score, node.CustomerId);
        if (cmp < 0) node.Left = Remove(node.Left, score, customerId);
        else if (cmp > 0) node.Right = Remove(node.Right, score, customerId);
        else
        {
            if (node.Left == null || node.Right == null)
            {
                var rep = node.Left ?? node.Right;
                if (rep != null) rep.Parent = node.Parent;
                return rep;
            }
            else
            {
                var succ = Min(node.Right);
                node.Score = succ.Score;
                node.CustomerId = succ.CustomerId;
                node.Right = Remove(node.Right, succ.Score, succ.CustomerId);
            }
        }
        return Balance(node);
    }
    private static Node Min(Node node)
    {
        while (node.Left != null) node = node.Left;
        return node;
    }
    private Node? Successor(Node node)
    {
        if (node.Right != null)
        {
            var cur = node.Right;
            while (cur.Left != null) cur = cur.Left;
            return cur;
        }
        var p = node.Parent; var c = node;
        while (p != null && c == p.Right) { c = p; p = p.Parent; }
        return p;
    }
    private Node? SelectByRank(Node node, int rank)
    {
        while (node != null)
        {
            int left = Count(node.Left);
            int nodeRank = left + 1;
            if (rank == nodeRank) return node;
            if (rank < nodeRank) node = node.Left!;
            else { rank -= nodeRank; node = node.Right!; }
        }
        return null;
    }
    private int GetRank(long customerId, decimal score)
    {
        var node = _root;
        int acc = 0;
        while (node != null)
        {
            int cmp = Compare(score, customerId, node.Score, node.CustomerId);
            if (cmp == 0) return acc + Count(node.Left) + 1;
            if (cmp < 0) node = node.Left;
            else
            {
                acc += Count(node.Left) + 1;
                node = node.Right;
            }
        }
        return -1;
    }
}