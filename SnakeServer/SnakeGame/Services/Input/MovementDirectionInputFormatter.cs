using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Models.Input.External;
using SnakeGame.Models.Input.Internal;

namespace SnakeGame.Services.Input;

internal class MovementDirectionInputFormatter(
    IEnumerable<IInputService<MovementDirectionInput>> services
    ) : IInputFormatter<BinaryInput>
{
    public bool TryResolve(BinaryInput input, ClientIdentifier id)
    {
        var model = new MovementDirectionInput()
        {
            Angle = BitConverter.ToSingle(input.Data)
        };
        foreach (var service in services)
        {
            service.OnInput(id, model);
        }
        return true;
    }


}
