using ServerEngine.Models;
using ServerEngine.Models.Input;

namespace ServerEngine.Interfaces;

public interface ISessionManager
{
    public Task<ISessionConnection> ConnectAsync(ClientIdentifier id);
    public void Close();
}