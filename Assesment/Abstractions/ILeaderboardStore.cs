public interface ILeaderboardStore
{
    // Upserts cumulative score; returns new score.
    decimal UpdateScore(long customerId, decimal delta);

    bool TryGetScore(long customerId, out decimal score);

    // Active = score > 0
    int ActiveCount { get; }

    // Rank is 1-based.
    LeaderboardEntry? GetByRank(int rank);

    // Returns rank or 0 if not active.
    int GetRank(long customerId);

    // Re-evaluates active membership after score change.
    void RefreshActive(long customerId, decimal newScore);
}