using ServerEngine.Models;

namespace ServerEngine.Interfaces;

public interface IInputFormatter<T>
{
    public bool TryResolve(T input, ClientIdentifier id);
}
