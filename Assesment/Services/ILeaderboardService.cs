namespace Assesment.Services;

using Assesment.Models;

public interface ILeaderboardService
{
    IEnumerable<LeaderboardEntry> GetAll();

    // 你已有的业务方法可继续保留（根据需要）
    decimal UpdateScore(long customerId, decimal delta);
    IReadOnlyList<LeaderboardEntry> GetRange(int startRank, int endRank);
    IReadOnlyList<LeaderboardEntry>? GetWithNeighbors(long customerId, int high, int low);
}