using GameTest.InputModels;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;

namespace GameTest.Services;

public class JumpReader : IInputService<JumpInput>
{
    public void OnInput(ClientIdentifier id, JumpInput data)
    {
        Console.WriteLine($"client '{id.Id}' jumped {data.Height}");
    }
}
