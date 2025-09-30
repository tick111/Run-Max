public static class ServiceRegistration
{
    public static IServiceCollection AddLeaderboardCore(this IServiceCollection services)
    {
        services.AddSingleton<ILeaderboardStore, InMemoryLeaderboardStore>();
        services.AddSingleton<ILeaderboardService, LeaderboardService>();
        return services;
    }
}