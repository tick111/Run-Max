namespace Assesment.Leaderboard;

public interface ILeaderboardService
{
    decimal UpdateScore(long customerId, decimal delta);
    IReadOnlyList<LeaderboardEntry> GetRange(int startRank, int endRank);
    IReadOnlyList<LeaderboardEntry>? GetWithNeighbors(long customerId, int high, int low);
}
