using ServerEngine.Interfaces;
using ServerEngine.Models;
using ServerEngine.Models.Input;
using System.Threading;

namespace ServerEngine;

internal class SessionManager : ISessionManager
{
    private readonly SessionHandler _handler;

    public SessionManager(SessionHandler handler)
    {
        _handler = handler;
    }

    public void Close()
    {
        _handler.Closed = true;
    }

    public async Task<ISessionConnection> ConnectAsync(ClientIdentifier id)
    {
        _handler.JoinQueue.Enqueue(id);
        await _handler.Semaphore.WaitAsync();
        _handler.Semaphore.Release();
        return new SessionConnection(_handler, id);
    }
}