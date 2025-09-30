namespace Assesment.Leaderboard;

public sealed class LeaderboardService : ILeaderboardService
{
    private readonly ILeaderboardStore _store;
    public LeaderboardService(ILeaderboardStore store)=>_store=store;

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
        if (startRank <=0 || endRank < startRank) return Array.Empty<LeaderboardEntry>();
        var list = new List<LeaderboardEntry>(Math.Max(0,endRank-startRank+1));
        for(int r=startRank;r<=endRank;r++){
            var e=_store.GetByRank(r); if(e==null) break; list.Add(e);
        }
        return list;
    }

    public IReadOnlyList<LeaderboardEntry>? GetWithNeighbors(long customerId, int high, int low)
    {
        int rank=_store.GetRank(customerId); if(rank==0) return null; if(high<0) high=0; if(low<0) low=0;
        int start=Math.Max(1,rank-high); int end=rank+low; var list=new List<LeaderboardEntry>(end-start+1);
        for(int r=start;r<=end;r++){ var e=_store.GetByRank(r); if(e==null) break; list.Add(e);} return list;
    }
}
