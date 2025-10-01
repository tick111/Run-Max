using Mywork.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ILeaderboardService, LeaderboardService>();
builder.Services.AddHealthChecks(); // Add this line

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();          // 已恢复 HTTPS 重定向
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// 可选健康检查（如不需要可删除）
app.MapGet("/health", () => Results.Ok("OK"));

// 1. 更新/增减分
app.MapPost("/customer/{customerId:long}/score/{delta}",
    (long customerId, string delta, ILeaderboardService svc) =>
{
    if (customerId <= 0) return Results.BadRequest("customerId must be > 0");
    if (!decimal.TryParse(delta, NumberStyles.Number, CultureInfo.InvariantCulture, out var d))
        return Results.BadRequest("delta invalid");
    if (d is < -1000m or > 1000m) return Results.BadRequest("delta must be in [-1000,1000]");
    var score = svc.UpdateScore(customerId, d);
    return Results.Ok(score);
});

// 2. 按排名区间
app.MapGet("/leaderboard", (int start, int end, ILeaderboardService svc) =>
{
    if (start < 1 || end < start) return Results.BadRequest("invalid rank range");
    return Results.Ok(svc.GetRange(start, end));
});

// 3. 指定客户及邻居
app.MapGet("/leaderboard/{customerId:long}",
    (long customerId, int? high, int? low, ILeaderboardService svc) =>
{
        int up = high ?? 0;
        int down = low ?? 0;
        if (up < 0 || down < 0) return Results.BadRequest("high/low must be >=0");
        var res = svc.GetWithNeighbors(customerId, up, down);
        return res is null ? Results.NotFound() : Results.Ok(res);
    });

app.Run();
