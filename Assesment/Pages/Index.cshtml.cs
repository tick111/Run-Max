using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
<<<<<<< HEAD
<<<<<<< HEAD

namespace Assesment.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        return RedirectToPage("/Leaderboard");
    }
}
=======
=======
>>>>>>> 865f3ae1f106c8950157245c6dd816e4b8b71140
using Assesment.Leaderboard;

namespace Assesment.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ILeaderboardService _leaderboard;

        public IndexModel(ILogger<IndexModel> logger, ILeaderboardService leaderboard)
        {
            _logger = logger;
            _leaderboard = leaderboard;
        }

        [BindProperty]
        public long CustomerId { get; set; }
        [BindProperty]
        public decimal Delta { get; set; }

        public int TopCount { get; private set; } = 20;
        public IReadOnlyList<LeaderboardEntry> TopEntries { get; private set; } = Array.Empty<LeaderboardEntry>();
        public string? Message { get; private set; }

        public void OnGet()
        {
            LoadTop();
        }

        public IActionResult OnPost()
        {
            try
            {
                _leaderboard.UpdateScore(CustomerId, Delta);
                Message = $"Updated customer {CustomerId} by {Delta}";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            LoadTop();
            return Page();
        }

        private void LoadTop()
        {
            TopEntries = _leaderboard.GetRange(1, TopCount);
        }
    }
}
<<<<<<< HEAD
>>>>>>> 865f3ae1f106c8950157245c6dd816e4b8b71140
=======
>>>>>>> 865f3ae1f106c8950157245c6dd816e4b8b71140
