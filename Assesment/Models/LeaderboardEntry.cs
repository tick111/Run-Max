namespace Assesment.Models;

public class LeaderboardEntry
{
    public long CustomerId { get; set; }
    public string UserName { get; set; } = "";
    public decimal Score { get; set; }
}