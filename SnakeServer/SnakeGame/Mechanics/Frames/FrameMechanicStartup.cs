using MessageSchemes;
using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Output;
using ServerEngine.Interfaces.Services;
using SnakeGame.Mechanics.Frames.Output;
using SnakeGame.Models.Output.External;

namespace SnakeGame.Mechanics.Frames;

internal static class FrameMechanicStartup
{
    public static void AddFrameMechanicRelated(this IServiceCollection services)
    {
        services.AddSingleton<FrameStorage>();
        services.AddSingleton<FrameRegistry>();
        services.AddSingleton<FrameRepository>();
        services.AddSingleton<INotificationListener>(provider => provider.GetRequiredService<FrameRepository>());
        services.AddSingleton<IMessageProvider>(provider => provider.GetRequiredService<FrameRepository>());
        services.AddSingleton<FrameFactory>();

        services.AddSingleton<IOutputService<EventMessage>, EventOutputService>();
        services.AddOutputFabricScoped<EventMessage, EventBasedBinaryOutput?, FrameToBinaryOutputTransformer>();
        services.AddScoped<IOutputProvider<StateBasedBinaryOutput>, StateOutputService>();
        services.AddOutputHandlerSingleton<StateBasedBinaryOutput>();
    }
}
