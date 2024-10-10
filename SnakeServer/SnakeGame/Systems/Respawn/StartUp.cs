﻿using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Interfaces.Services;
using SnakeGame.Mechanics.Respawn;
using SnakeGame.Models.Input.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Systems.Respawn;

internal static class StartUp
{
    public static void AddRespawn(this IServiceCollection services)
    {
        services.AddSingleton<RespawnManager>();
        services.AddSingleton<IInputService<OptionInput>>(provider => provider.GetRequiredService<RespawnManager>());
        services.AddSingleton<ISessionService>(provider => provider.GetRequiredService<RespawnManager>());
        services.AddSingleton<IUpdateService>(provider => provider.GetRequiredService<RespawnManager>());
        services.AddSingleton<IRespawnEventSource>(provider => provider.GetRequiredService<RespawnManager>());
    }

    public static void AddRespawnEventAggregator(this IServiceCollection services)
    {
        services.AddSingleton<IStartUpService, RespawnEventAggregator>();
    }
}
