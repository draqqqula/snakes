using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Interfaces.Services;
using SnakeGame.Services.Output;
using SnakeGame.Systems.Statistics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Systems.Statistics;

internal static class StartUp
{
    public static void AddRuntimeCommands(this IServiceCollection services)
    {
        services.AddSingleton<RuntimeCommandAggregator>();
        services.AddSingleton<IRuntimeCommandAggregator>(provider => provider.GetRequiredService<RuntimeCommandAggregator>());
        services.AddSingleton<ISessionService>(provider => provider.GetRequiredService<RuntimeCommandAggregator>());
        services.AddSingleton<IOutputService<ClientCommandWrapper>>(provider => provider.GetRequiredService<RuntimeCommandAggregator>());
        services.AddSingleton<IRuntimeCommandFactory, RuntimeCommandStartUpFactory>();
    }
}
