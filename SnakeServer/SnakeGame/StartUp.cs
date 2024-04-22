using Microsoft.Extensions.DependencyInjection;
using ServerEngine;
using ServerEngine.Interfaces;
using SnakeGame.Common;

namespace SnakeGame;

public static class StartUp
{
    public static void AddGameApplication(this IServiceCollection services)
    {
        var gameApplication = ApplicationBuilder.BuildApplication();
        services.AddSingleton(gameApplication);
        services.AddSingleton<ISessionStorage<Guid>, SessionStorage>();
    }

    public static void AddGameLauncher(this IServiceCollection services)
    {
        services.AddSingleton<ISessionLauncher, GameLauncher>();
    }
}
