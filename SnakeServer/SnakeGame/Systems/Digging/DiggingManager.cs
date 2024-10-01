using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Systems.GameObjects.Characters;
using SnakeGame.Systems.Timer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Systems.Digging;

internal class DiggingManager(Terrain Terrain, Dictionary<ClientIdentifier, SnakeCharacter> Snakes, ITimerScheduler TimerScheduler) : IUpdateService
{
    public void Update(IGameContext context)
    {
        foreach (var snake in Snakes.Values)
        {
            if (Terrain.TryDig(new Circle() 
            { 
                Position = snake.Head.Transform.Position, 
                Radius = 5 
            }))
            {
                snake.Speed = 20f;
            }
        }
    }
}
