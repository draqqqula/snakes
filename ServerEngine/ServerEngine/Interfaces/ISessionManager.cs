using ServerEngine.Models;
using ServerEngine.Models.Input;

namespace ServerEngine.Interfaces;

public interface ISessionManager
{
    public event Action OnClosed;
    public Task<ISessionConnection> ConnectAsync(ClientIdentifier id);
    public void Close();
    public SessionStatus GetStatus();
}