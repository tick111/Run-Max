using Mywork.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ILeaderboardService, LeaderboardService>();
builder.Services.AddCors(o=>o.AddPolicy("Front",
    p=>p.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("Front");

// 可选：根路径返回说明，避免 404 误会
app.MapGet("/", () => Results.Text("Backend OK. Use /api/leaderboard"));

// 排行接口：支持排序
app.MapGet("/api/leaderboard", (int? start, int? end, string? sort, string? order, ILeaderboardService svc) =>
{
    int s = start ?? 1;
    int e = end ?? (s + 19);
    if (s < 1 || e < s) return Results.BadRequest("invalid range");
    var list = svc.GetRange(s, e);

    // 排序
    var dirDesc = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
    IEnumerable<dynamic> q = list;

    switch ((sort ?? "").ToLowerInvariant())
    {
        case "score":
            q = dirDesc ? list.OrderByDescending(x => x.Score)
                        : list.OrderBy(x => x.Score);
            break;
        case "user":
        case "username":
            q = dirDesc ? list.OrderByDescending(x => x.UserName)
                        : list.OrderBy(x => x.UserName);
            break;
        case "rank":
        default:
            q = dirDesc ? list.OrderByDescending(x => x.Rank)
                        : list.OrderBy(x => x.Rank);
            break;
    }

    // 重新给 Rank（排序后想保留原 rank 可不改）
    var result = q.Select((x,i) => new {
        rank = i + 1,
        customerId = x.CustomerId,
        userName = x.UserName,
        score = x.Score
    });

    return Results.Ok(result);
});

// 增减分接口
app.MapPost("/api/customer/{customerId:long}/score/{delta}",
    (long customerId,string delta,ILeaderboardService svc)=>{
        if(!decimal.TryParse(delta, NumberStyles.Number,
            CultureInfo.InvariantCulture,out var d))
            return Results.BadRequest("delta invalid");
        var newScore = svc.UpdateScore(customerId,d);
        return Results.Ok(new { customerId, score=newScore });
    });

app.Run();
