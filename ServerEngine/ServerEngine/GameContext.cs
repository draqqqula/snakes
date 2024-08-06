﻿using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Interfaces;

namespace ServerEngine;

internal class GameContext(IServiceProvider provider) : IGameContext
{
    private readonly IServiceProvider _provider = provider;

    public float DeltaTime { get; internal set; }

    public T Using<T>()
    {
        return _provider.GetRequiredService<T>();
    }
}