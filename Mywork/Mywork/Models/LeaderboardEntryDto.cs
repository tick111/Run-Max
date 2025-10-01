namespace Mywork.Models;

public sealed record LeaderboardEntryDto(long CustomerId, decimal Score, int Rank);