namespace Assesment.Leaderboard;

public interface ILeaderboardService
{
    decimal UpdateScore(long customerId, decimal delta);
    List<LeaderboardEntryDto> GetRange(int start, int end);
    List<LeaderboardEntryDto>? GetWithNeighbors(long customerId, int high, int low);
}