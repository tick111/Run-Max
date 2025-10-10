var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLeaderboardCore();
builder.Services.AddRazorPages();

// 精准允许前端源（推荐）
builder.Services.AddCors(o => o.AddPolicy("Front",
    p => p.WithOrigins("http://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod()
));

var app = builder.Build();

app.UseHttpsRedirection();

// 若有 UseRouting 则： app.UseRouting();
app.UseCors("Front"); // 放在 MapRazorPages / MapGet 之前

app.MapRazorPages();

// 如果 /leaderboard-ui 是静态文件(非 Razor)，需为静态文件加头：
// app.UseStaticFiles(new StaticFileOptions {
//     OnPrepareResponse = ctx => {
//         ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");
//     }
// });

app.Run();