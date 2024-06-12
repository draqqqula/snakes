using MessageSchemes;
using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Interfaces.Output;
using ServerEngine.Interfaces.Services;
using SnakeGame.Mechanics.Frames.Output.Interfaces;
using SnakeGame.Mechanics.Frames.Output;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Models.Output.External;
using ServerEngine.Interfaces;
using ServerEngine.Models;
using SnakeGame.Models.Output.Internal;
using SnakeGame.Services.Output.Services;
using SnakeGame.Services.Output;

namespace SnakeGame.Mechanics.ViewPort;

internal static class StartUp
{
    public static void AddViewProduction(this IServiceCollection services)
    {
        services.AddSingleton<Dictionary<ClientIdentifier, ViewPort>>();
        services.AddSingleton<ITableProvider>(provider => provider.GetRequiredService<FrameRepository>());
        services.AddSingleton<ViewPortManager>();
        services.AddSingleton<IUpdateService, ViewPortToCharacterBinder>();
        services.AddSingleton<IUpdateService>(provider => provider.GetRequiredService<ViewPortManager>());
        services.AddSingleton<ISessionService>(provider => provider.GetRequiredService<ViewPortManager>());
    }
}
