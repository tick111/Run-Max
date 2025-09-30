using System.Collections.Concurrent;
using System.Threading;

namespace Assesment.Leaderboard;

internal sealed class InMemoryLeaderboardStore : ILeaderboardStore
{
    private readonly ConcurrentDictionary<long, decimal> _scores = new();
    private readonly OrderStatisticAvlTree _tree = new();
    private readonly Dictionary<long, OrderStatisticAvlTree.Node> _active = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public int ActiveCount { get { _lock.EnterReadLock(); try { return _tree.Count; } finally { _lock.ExitReadLock(); } } }

    public decimal UpdateScore(long customerId, decimal delta) => _scores.AddOrUpdate(customerId, delta, (_, old) => old + delta);
    public bool TryGetScore(long customerId, out decimal score) => _scores.TryGetValue(customerId, out score);

    public void RefreshActive(long customerId, decimal newScore)
    {
        _lock.EnterWriteLock();
        try
        {
            bool active = _active.TryGetValue(customerId, out var node);
            bool shouldBe = newScore > 0m;
            if (active && !shouldBe)
            { _tree.Remove(node!); _active.Remove(customerId); }
            else if (!active && shouldBe)
            { var n = _tree.Insert(customerId, newScore); _active[customerId] = n; }
            else if (active && shouldBe)
            { _tree.Remove(node!); var n = _tree.Insert(customerId, newScore); _active[customerId] = n; }
        }
        finally { _lock.ExitWriteLock(); }
    }

    public LeaderboardEntry? GetByRank(int rank)
    {
        _lock.EnterReadLock();
        try
        {
            var node = _tree.SelectByRank(rank); if (node == null) return null; return new LeaderboardEntry(node.CustomerId, node.Score, rank);
        }
        finally { _lock.ExitReadLock(); }
    }

    public int GetRank(long customerId)
    {
        _lock.EnterReadLock();
        try { if (!_active.TryGetValue(customerId, out var node)) return 0; return _tree.GetRank(node); }
        finally { _lock.ExitReadLock(); }
    }
}
