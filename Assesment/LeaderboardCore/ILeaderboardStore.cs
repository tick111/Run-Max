namespace Assesment.Leaderboard;

public interface ILeaderboardStore
{
    decimal UpdateScore(long customerId, decimal delta);
    bool TryGetScore(long customerId, out decimal score);
    int ActiveCount { get; }
    LeaderboardEntry? GetByRank(int rank);
    int GetRank(long customerId);
    void RefreshActive(long customerId, decimal newScore);
}
