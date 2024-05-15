using MessageSchemes;
using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Interfaces.Output;
using ServerEngine.Interfaces.Services;
using SnakeGame.Mechanics.Frames.Output.Interfaces;
using SnakeGame.Mechanics.Frames.Output;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Models.Output.External;
using SnakeGame.Mechanics.ViewPort.Output;
using ServerEngine.Interfaces;
using ServerEngine.Models;

namespace SnakeGame.Mechanics.ViewPort;

internal static class StartUp
{
    public static void AddViewProduction(this IServiceCollection services)
    {
        services.AddSingleton<Dictionary<ClientIdentifier, ViewPort>>();
        services.AddSingleton<ITableProvider>(provider => provider.GetRequiredService<FrameRepository>());
        services.AddSingleton<ViewPortManager>();
        services.AddSingleton<IUpdateService>(provider => provider.GetRequiredService<ViewPortManager>());
        services.AddSingleton<ISessionService>(provider => provider.GetRequiredService<ViewPortManager>());
        services.AddSingleton<IUpdateService, ViewPortToCharacterBinder>();
        services.AddSingleton<IOutputService<ClientEventSet>, ViewOutputService>();
        services.AddOutputFabricScoped<ClientEventSet, ViewPortBasedBinaryOutput, ViewPortBasedOutputTransformer>();
    }
}
