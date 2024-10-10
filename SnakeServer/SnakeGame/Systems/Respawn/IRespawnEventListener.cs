using SnakeGame.Systems.GameObjects.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Systems.Respawn;

internal interface IRespawnEventListener
{
    public void OnRespawn(SnakeCharacter snake);
}
