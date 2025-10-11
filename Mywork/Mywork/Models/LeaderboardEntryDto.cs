namespace Mywork.Models;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public long CustomerId { get; set; }
    public string UserName { get; set; } = "";
    public decimal Score { get; set; }
}