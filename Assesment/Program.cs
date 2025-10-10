var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLeaderboardCore();
builder.Services.AddRazorPages();

// 允许前端 http://localhost:3000
builder.Services.AddCors(o => o.AddPolicy("Front",
    p => p.WithOrigins("http://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod()
          // 如果以后需要带凭据再加 .AllowCredentials()
));

var app = builder.Build();

app.UseHttpsRedirection();

// 如果你显式使用路由中间件： app.UseRouting();
app.UseCors("Front"); // 必须在 MapRazorPages / MapGet 之前

app.MapRazorPages();

// 如果 /leaderboard-ui/ 对应的是纯静态文件目录(非 Razor Page)，需要允许静态文件 CORS：
/*
app.UseStaticFiles(new StaticFileOptions {
    OnPrepareResponse = ctx => {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");
        ctx.Context.Response.Headers.Append("Vary", "Origin");
    }
});
*/

app.Run();