using System.Collections.Concurrent;
using Assesment.Models;

namespace Assesment.Services;

public sealed class LeaderboardService
{
    private readonly RankedSkipList _skipList = new();
    private readonly ConcurrentDictionary<long, decimal> _scores = new();

    public decimal AddOrUpdate(long customerId, decimal delta)
    {
        var newScore = _scores.AddOrUpdate(customerId, delta, (_, old) => old + delta);
        if (newScore > 0)
            _skipList.InsertOrUpdate(customerId, newScore);
        else
            _skipList.Remove(customerId);
        return newScore;
    }

    public List<LeaderboardEntryDto> GetRangeByRank(int start, int end)
    {
        var data = _skipList.GetRangeByRank(start, end);
        var list = new List<LeaderboardEntryDto>(data.Count);
        foreach (var x in data)
            list.Add(new LeaderboardEntryDto(x.CustomerId, x.Score, x.Rank));
        return list;
    }

    public List<LeaderboardEntryDto>? GetWithNeighbors(long customerId, int high, int low)
    {
        var node = _skipList.GetNode(customerId);
        if (node == null) return null;
        int rank = _skipList.GetRank(node.Score, customerId);
        if (rank <= 0) return null;

        int start = Math.Max(1, rank - high);
        int end = rank + low;
        var range = _skipList.GetRangeByRank(start, end);

        var list = new List<LeaderboardEntryDto>(range.Count);
        foreach (var x in range)
            list.Add(new LeaderboardEntryDto(x.CustomerId, x.Score, x.Rank));
        return list;
    }
}