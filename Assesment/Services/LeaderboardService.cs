using Assesment.Models;

public sealed class LeaderboardService : ILeaderboardService
{
    private readonly ILeaderboardStore _store;

    public LeaderboardService(ILeaderboardStore store) => _store = store;

    // 新增：演示用，遍历一个合理范围（如前 100 名）
    public IEnumerable<LeaderboardEntry> GetAll()
    {
        for (int rank = 1; rank <= 100; rank++)
        {
            var e = _store.GetByRank(rank);
            if (e == null) break;
            yield return e;
        }
    }

    public decimal UpdateScore(long customerId, decimal delta)
    {
        if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));
        if (delta < -1000m || delta > 1000m) throw new ArgumentOutOfRangeException(nameof(delta));

        var newScore = _store.UpdateScore(customerId, delta);
        _store.RefreshActive(customerId, newScore);
        return newScore;
    }

    public IReadOnlyList<LeaderboardEntry> GetRange(int startRank, int endRank)
    {
        if (startRank <= 0 || endRank < startRank) return Array.Empty<LeaderboardEntry>();
        var list = new List<LeaderboardEntry>(Math.Max(0, endRank - startRank + 1));
        for (int r = startRank; r <= endRank; r++)
        {
            var entry = _store.GetByRank(r);
            if (entry == null) break;
            list.Add(entry);
        }
        return list;
    }

    public IReadOnlyList<LeaderboardEntry>? GetWithNeighbors(long customerId, int high, int low)
    {
        int rank = _store.GetRank(customerId);
        if (rank == 0) return null;

        int start = Math.Max(1, rank - high);
        int end = rank + low;

        var list = new List<LeaderboardEntry>(end - start + 1);
        for (int r = start; r <= end; r++)
        {
            var e = _store.GetByRank(r);
            if (e == null) break;
            list.Add(e);
        }
        return list;
    }
}