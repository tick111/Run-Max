using Mywork.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ILeaderboardService, LeaderboardService>();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// 健康检查
app.MapGet("/health", () => Results.Ok("OK"));

// 1. 更新/增减分 (API 前缀)
app.MapPost("/api/customer/{customerId:long}/score/{delta}",
    (long customerId, string delta, ILeaderboardService svc) =>
{
    if (customerId <= 0) return Results.BadRequest("customerId must be > 0");
    if (!decimal.TryParse(delta, NumberStyles.Number, CultureInfo.InvariantCulture, out var d))
        return Results.BadRequest("delta invalid");
    if (d is < -1000m or > 1000m) return Results.BadRequest("delta must be in [-1000,1000]");
    var score = svc.UpdateScore(customerId, d);
    return Results.Ok(new { customerId, score });
});

// 2. 按排名区间（start/end 可选 + 默认）
app.MapGet("/api/leaderboard", (int? start, int? end, ILeaderboardService svc) =>
{
    try
    {
        int s = start.GetValueOrDefault(1);
        int e = end.GetValueOrDefault(s + 49);
        if (s < 1 || e < s) return Results.BadRequest("invalid rank range");
        var list = svc.GetRange(s, e).ToList();
        return Results.Ok(list);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// 3. 指定客户及邻居
app.MapGet("/api/leaderboard/{customerId:long}",
    (long customerId, int? high, int? low, ILeaderboardService svc) =>
{
    int up = high.GetValueOrDefault(0);
    int down = low.GetValueOrDefault(0);
    if (up < 0 || down < 0) return Results.BadRequest("high/low must be >=0");
    var res = svc.GetWithNeighbors(customerId, up, down);
    return res is null ? Results.NotFound() : Results.Ok(res);
});

app.Run();
