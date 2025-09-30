using Microsoft.AspNetCore.Mvc;
using Assesment.Services;
using Assesment.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<LeaderboardService>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

// POST /api/customer/{customerId}/score/{delta}
app.MapPost("/api/customer/{customerId:long}/score/{delta:decimal}", (
    [FromRoute] long customerId,
    [FromRoute(Name="delta")] decimal delta,
    LeaderboardService svc) =>
{
    if (customerId <= 0) return Results.BadRequest("customerId must be positive.");
    if (delta is < -1000m or > 1000m) return Results.BadRequest("delta out of range [-1000,1000].");
    var current = svc.AddOrUpdate(customerId, delta);
    return Results.Ok(current);
});

// GET /api/leaderboard?start=&end=
app.MapGet("/api/leaderboard", (
    [FromQuery] int? start,
    [FromQuery] int? end,
    LeaderboardService svc) =>
{
    int s = start.GetValueOrDefault(1);
    int e = end.GetValueOrDefault(Math.Max(s, s + 19));
    if (s <= 0 || e <= 0 || e < s) return Results.BadRequest("Invalid start/end.");
    return Results.Ok(svc.GetRangeByRank(s, e));
});

// GET /api/leaderboard/{customerId}?high=&low=
app.MapGet("/api/leaderboard/{customerId:long}", (
    [FromRoute] long customerId,
    [FromQuery] int? high,
    [FromQuery] int? low,
    LeaderboardService svc) =>
{
    if (customerId <= 0) return Results.BadRequest("customerId must be positive.");
    int hi = Math.Max(0, high ?? 0);
    int lo = Math.Max(0, low ?? 0);
    var result = svc.GetWithNeighbors(customerId, hi, lo);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.Run();
