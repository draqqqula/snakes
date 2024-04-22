using GameTest.InputModels;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;

namespace GameTest.Services;

public class TestClientService : ISessionService, IInputService<MovementInput>
{
    public void OnInput(ClientIdentifier id, MovementInput data)
    {
        Console.WriteLine($"client '{id.Id}' moved {data.Distance}");
    }

    public void OnJoin(IGameContext context, ClientIdentifier id)
    {
        Console.WriteLine($"client '{id.Id}' joined");
    }

    public void OnLeave(IGameContext context, ClientIdentifier id)
    {
        Console.WriteLine($"client '{id.Id}' left");
    }
}
