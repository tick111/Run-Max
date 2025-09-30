using Assesment.Leaderboard;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register leaderboard core services (infrastructure + service abstraction)
builder.Services.AddLeaderboardCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Minimal API endpoints for leaderboard operations
app.MapPost("/customer/{customerId:long}/score/{delta:decimal}", (long customerId, decimal delta, ILeaderboardService svc) =>
{
    try
    {
        var updated = svc.UpdateScore(customerId, delta);
        return Results.Ok(new { customerId, score = updated });
    }
    catch (ArgumentOutOfRangeException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/leaderboard", (int start, int end, ILeaderboardService svc) =>
{
    var entries = svc.GetRange(start, end);
    return Results.Ok(entries);
});

app.MapGet("/leaderboard/{customerId:long}", (long customerId, int? high, int? low, ILeaderboardService svc) =>
{
    var entries = svc.GetWithNeighbors(customerId, high ?? 0, low ?? 0);
    if (entries == null || entries.Count == 0) return Results.NotFound();
    return Results.Ok(entries);
});

app.Run();
