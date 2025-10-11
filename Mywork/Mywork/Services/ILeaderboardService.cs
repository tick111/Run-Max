using Mywork.Models;

namespace Mywork.Services;

public interface ILeaderboardService
{
    IReadOnlyList<LeaderboardEntryDto> GetRange(int start, int end);
    IReadOnlyList<LeaderboardEntryDto>? GetWithNeighbors(long customerId, int high, int low);
    decimal UpdateScore(long customerId, decimal delta);
}