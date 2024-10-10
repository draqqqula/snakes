using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Systems.Respawn;

internal class RespawnEventAggregator(
    IRespawnEventSource source,
    IEnumerable<IRespawnEventListener> listeners) : IStartUpService
{
    public void Start(IGameContext context)
    {
        foreach (var listener in listeners)
        {
            source.OnRespawn += listener.OnRespawn;
        }
    }
}
