using System;
using System.Collections.Generic;
using System.Threading;

namespace Assesment.Services;

internal sealed class RankedSkipList
{
    private const int MaxLevel = 32;
    private const double Probability = 0.25;

    private readonly Node _head;
    private int _level;
    private int _length;
    private readonly Dictionary<long, Node> _byId = new();
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);

    private readonly ThreadLocal<Random> _rand = new(() => new Random(Guid.NewGuid().GetHashCode()));

    public RankedSkipList()
    {
        _head = new Node(0, 0, 0, MaxLevel);
        _level = 1;
        _length = 0;
    }

    private int RandomLevel()
    {
        int lvl = 1;
        while (lvl < MaxLevel && _rand.Value!.NextDouble() < Probability)
            lvl++;
        return lvl;
    }

    private static int Compare(decimal scoreA, long idA, decimal scoreB, long idB)
    {
        int c = scoreB.CompareTo(scoreA);
        if (c != 0) return c;
        return idA.CompareTo(idB);
    }

    public void InsertOrUpdate(long customerId, decimal score)
    {
        _lock.EnterWriteLock();
        try
        {
            if (_byId.TryGetValue(customerId, out var existing))
            {
                if (existing.Score == score) return;
                RemoveInternal(existing, false);
                InsertInternal(customerId, score);
            }
            else
            {
                InsertInternal(customerId, score);
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Remove(long customerId)
    {
        _lock.EnterWriteLock();
        try
        {
            if (_byId.TryGetValue(customerId, out var node))
                RemoveInternal(node, true);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    private void InsertInternal(long customerId, decimal score)
    {
        var update = new Node[MaxLevel];
        var rank = new int[MaxLevel];

        var x = _head;
        for (int i = _level - 1; i >= 0; i--)
        {
            rank[i] = i == _level - 1 ? 0 : rank[i + 1];
            while (x.Forward[i] != null && Compare(x.Forward[i]!.Score, x.Forward[i]!.CustomerId, score, customerId) < 0)
            {
                rank[i] += x.Span[i];
                x = x.Forward[i]!;
            }
            update[i] = x;
        }

        int lvl = RandomLevel();
        if (lvl > _level)
        {
            for (int i = _level; i < lvl; i++)
            {
                update[i] = _head;
                update[i].Span[i] = _length;
                rank[i] = 0;
            }
            _level = lvl;
        }

        var node = new Node(customerId, score, lvl, MaxLevel);
        for (int i = 0; i < lvl; i++)
        {
            node.Forward[i] = update[i].Forward[i];
            update[i].Forward[i] = node;

            node.Span[i] = update[i].Span[i] - (rank[0] - rank[i]);
            update[i].Span[i] = (rank[0] - rank[i]) + 1;
        }

        for (int i = lvl; i < _level; i++)
            update[i].Span[i]++;

        node.Backward = update[0] == _head ? null : update[0];
        if (node.Forward[0] != null)
            node.Forward[0]!.Backward = node;

        _length++;
        _byId[customerId] = node;
    }

    private void RemoveInternal(Node node, bool removeFromDict)
    {
        var update = new Node[MaxLevel];
        var x = _head;
        for (int i = _level - 1; i >= 0; i--)
        {
            while (x.Forward[i] != null && !ReferenceEquals(x.Forward[i], node) &&
                   Compare(x.Forward[i]!.Score, x.Forward[i]!.CustomerId, node.Score, node.CustomerId) < 0)
            {
                x = x.Forward[i]!;
            }
            update[i] = x;
        }

        for (int i = 0; i < _level; i++)
        {
            if (update[i].Forward[i] == node)
            {
                update[i].Span[i] += node.Span[i] - 1;
                update[i].Forward[i] = node.Forward[i];
            }
            else
            {
                update[i].Span[i]--;
            }
        }

        if (node.Forward[0] != null)
            node.Forward[0]!.Backward = node.Backward;

        while (_level > 1 && _head.Forward[_level - 1] == null)
            _level--;

        _length--;
        if (removeFromDict)
            _byId.Remove(node.CustomerId);
    }

    public int GetRank(decimal score, long customerId)
    {
        _lock.EnterReadLock();
        try
        {
            int rank = 0;
            var x = _head;
            for (int i = _level - 1; i >= 0; i--)
            {
                while (x.Forward[i] != null &&
                       Compare(x.Forward[i]!.Score, x.Forward[i]!.CustomerId, score, customerId) < 0)
                {
                    rank += x.Span[i];
                    x = x.Forward[i]!;
                }
            }
            if (x.Forward[0] != null &&
                x.Forward[0]!.Score == score &&
                x.Forward[0]!.CustomerId == customerId)
            {
                rank += 1;
                return rank;
            }
            return -1;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public Node? GetNode(long customerId)
    {
        _lock.EnterReadLock();
        try
        {
            _byId.TryGetValue(customerId, out var node);
            return node;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public List<(long CustomerId, decimal Score, int Rank)> GetRangeByRank(int start, int end)
    {
        var result = new List<(long, decimal, int)>();
        _lock.EnterReadLock();
        try
        {
            if (start > _length) return result;
            if (end > _length) end = _length;
            if (start > end) return result;

            int traversed = 0;
            var x = _head;
            for (int i = _level - 1; i >= 0; i--)
            {
                while (x.Forward[i] != null && (traversed + x.Span[i]) < start)
                {
                    traversed += x.Span[i];
                    x = x.Forward[i]!;
                }
            }

            x = x.Forward[0]!;
            int rank = traversed + 1;
            while (x != null && rank <= end)
            {
                result.Add((x.CustomerId, x.Score, rank));
                x = x.Forward[0];
                rank++;
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }
        return result;
    }

    internal sealed class Node
    {
        public long CustomerId { get; }
        public decimal Score { get; }
        public Node?[] Forward { get; }
        public int[] Span { get; }
        public Node? Backward { get; set; }
        public int Level { get; }

        public Node(long customerId, decimal score, int nodeLevel, int maxLevel)
        {
            CustomerId = customerId;
            Score = score;
            Level = nodeLevel;
            Forward = new Node?[maxLevel];
            Span = new int[maxLevel];
            for (int i = 0; i < maxLevel; i++)
                Span[i] = 1;
        }
    }
}