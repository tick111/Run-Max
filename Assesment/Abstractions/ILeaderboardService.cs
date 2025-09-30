public interface ILeaderboardService
{
    decimal UpdateScore(long customerId, decimal delta);

    // Inclusive ranks
    IReadOnlyList<LeaderboardEntry> GetRange(int startRank, int endRank);

    // Returns target + neighbors
    IReadOnlyList<LeaderboardEntry>? GetWithNeighbors(long customerId, int high, int low);
}
