using Mywork.Models;

namespace Mywork.Services;

public interface ILeaderboardService
{
    decimal UpdateScore(long customerId, decimal delta);
    IEnumerable<LeaderboardEntryDto> GetRange(int startRank, int endRank);
    IEnumerable<LeaderboardEntryDto>? GetWithNeighbors(long customerId, int high, int low);
}