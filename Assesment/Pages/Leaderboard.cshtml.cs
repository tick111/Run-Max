using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assesment.Services;
using Assesment.Models;

namespace Assesment.Pages;

public class LeaderboardModel : PageModel
{
    private readonly LeaderboardService _svc;
    public List<LeaderboardEntryDto> Range { get; private set; } = new();
    public List<LeaderboardEntryDto>? Focus { get; private set; }

    [BindProperty] public long UpdateCustomerId { get; set; }
    [BindProperty] public decimal Delta { get; set; }
    [BindProperty] public int Start { get; set; } = 1;
    [BindProperty] public int End { get; set; } = 20;
    [BindProperty] public long LookupCustomerId { get; set; }
    [BindProperty] public int High { get; set; } = 2;
    [BindProperty] public int Low { get; set; } = 2;

    public LeaderboardModel(LeaderboardService svc) => _svc = svc;

    public void OnGet() => Range = _svc.GetRangeByRank(Start, End);

    public IActionResult OnPostUpdate()
    {
        if (UpdateCustomerId > 0 && Delta is >= -1000 and <= 1000)
            _svc.AddOrUpdate(UpdateCustomerId, Delta);
        return RedirectToPage(new { Start, End });
    }

    public IActionResult OnPostQueryRange() => RedirectToPage(new { Start, End });

    public IActionResult OnPostFocus()
    {
        if (LookupCustomerId > 0)
            Focus = _svc.GetWithNeighbors(LookupCustomerId, High, Low);
        Range = _svc.GetRangeByRank(Start, End);
        return Page();
    }
}

