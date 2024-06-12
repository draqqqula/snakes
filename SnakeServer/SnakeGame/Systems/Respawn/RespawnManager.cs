using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Models.Input.Internal;
using SnakeGame.Services;
using SnakeGame.Services.Gameplay.FrameDrivers;
using SnakeGame.Services.Output.Commands;
using SnakeGame.Systems.Respawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Mechanics.Respawn;

internal class RespawnManager(

    SnakeSpawner Spawner,
    IGameConfiguration Configuration,
    IEnumerable<IAbilityFactory> Factories,
    CommandSender Sender

    ) : ISessionService, IInputService<OptionInput>, IUpdateService
{
    private readonly TimeSpan RespawnTime = Configuration.Get<TimeSpan>(nameof(RespawnTime));

    private readonly Dictionary<ClientIdentifier, IAbilityFactory> _selectedAbilities = [];

    private readonly Dictionary<ClientIdentifier, RespawnTimer> _awaitingInput = [];

    private readonly IAbilityFactory[] _availableAbilities = Factories.ToArray();

    public void QueueRespawn(ClientIdentifier id)
    {
        var timer = RespawnTimer.Set(RespawnTime);
        _awaitingInput.Add(id, timer);
        ShowRespawnMenuCommand.To(id, Sender, RespawnTime);
    }

    private void Respawn(ClientIdentifier id)
    {
        Spawner.Spawn(id, _selectedAbilities[id]);
    }

    public void OnInput(ClientIdentifier id, OptionInput data)
    {
        if (_awaitingInput.ContainsKey(id))
        {
            if (data.OptionIndex < 0 || data.OptionIndex >= _availableAbilities.Length)
            {
                return;
            }
            _selectedAbilities[id] = _availableAbilities[data.OptionIndex];
        }
    }

    public void OnJoin(IGameContext context, ClientIdentifier id)
    {
        QueueRespawn(id);
        _selectedAbilities.Add(id, _availableAbilities.First());
    }

    public void OnLeave(IGameContext context, ClientIdentifier id)
    {
        _awaitingInput.Remove(id);
        _selectedAbilities.Remove(id);
    }

    public void Update(IGameContext context)
    {
        foreach (var timer in _awaitingInput.ToArray())
        {
            if (timer.Value.Expired)
            {
                Respawn(timer.Key);
                _awaitingInput.Remove(timer.Key);
            }
        }
    }
}
