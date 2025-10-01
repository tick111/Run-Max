using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LeaderboardModel : PageModel
{
    private readonly AppDbContext _context;

    public LeaderboardModel(AppDbContext context)
    {
        _context = context;
    }

    public List<CustomerLeaderboardEntry> Leaderboard { get; set; }

    public async Task OnGetAsync()
    {
        Leaderboard = await _context.Customers
            .Where(c => c.Score > 0)
            .OrderByDescending(c => c.Score)
            .ThenBy(c => c.CustomerID)
            .Select((c, index) => new CustomerLeaderboardEntry
            {
                Rank = index + 1,
                CustomerID = c.CustomerID,
                Score = c.Score
            })
            .ToListAsync();
    }

    public class CustomerLeaderboardEntry
    {
        public int Rank { get; set; }
        public long CustomerID { get; set; }
        public decimal Score { get; set; }
    }
}