var builder = WebApplication.CreateBuilder(args);

// Core leaderboard (framework only, no endpoints yet)
builder.Services.AddLeaderboardCore();

// Razor Pages still available if needed
builder.Services.AddRazorPages();

var app = builder.Build();
app.MapRazorPages();
app.Run();