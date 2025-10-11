using System.Collections.Generic;
using System.Linq;
using Mywork.Models;

namespace Mywork.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly object _lock = new();
    private readonly Dictionary<long, LeaderboardEntryDto> _byId;
    private readonly List<LeaderboardEntryDto> _ordered;

    public LeaderboardService()
    {
        _ordered = Enumerable.Range(1,50).Select(i => new LeaderboardEntryDto {
            Rank = i,
            CustomerId = 1000 + i,
            UserName = "User" + i,
            Score = 1000 - i * 3
        }).ToList();
        _byId = _ordered.ToDictionary(x=>x.CustomerId);
    }

    public IReadOnlyList<LeaderboardEntryDto> GetRange(int start, int end)
    {
        if (start < 1) start = 1;
        if (end < start) return Array.Empty<LeaderboardEntryDto>();
        return _ordered.Where(x => x.Rank >= start && x.Rank <= end).ToList();
    }

    public IReadOnlyList<LeaderboardEntryDto>? GetWithNeighbors(long customerId, int high, int low)
    {
        if (!_byId.TryGetValue(customerId, out var center)) return null;
        int s = Math.Max(1, center.Rank - high);
        int e = Math.Min(_ordered.Count, center.Rank + low);
        return _ordered.Where(x => x.Rank >= s && x.Rank <= e).ToList();
    }

    public decimal UpdateScore(long customerId, decimal delta)
    {
        lock(_lock)
        {
            if (!_byId.TryGetValue(customerId, out var entry))
            {
                entry = new LeaderboardEntryDto {
                    CustomerId = customerId,
                    UserName = "User" + customerId,
                    Score = 0
                };
                _byId[customerId] = entry;
                _ordered.Add(entry);
            }
            entry.Score += delta;
            // 重新排序
            _ordered.Sort((a,b)=> b.Score.CompareTo(a.Score));
            for(int i=0;i<_ordered.Count;i++) _ordered[i].Rank = i+1;
            return entry.Score;
        }
    }
}