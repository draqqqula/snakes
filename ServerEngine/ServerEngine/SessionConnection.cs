using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Output;
using ServerEngine.Models;
using ServerEngine.Models.Input;

namespace ServerEngine;

internal class SessionConnection(SessionHandler handler, ClientIdentifier id) : ISessionConnection
{
    private readonly ClientIdentifier _id = id;
    private readonly SessionHandler _handler = handler;

    public bool Closed { get; private set; } = false;

    private void Disconnect()
    {
        _handler.LeaveQueue.Enqueue(_id);
        Closed = true;
    }

    public void Dispose()
    {
        Disconnect();
    }

    public async Task<T?> GetOutputAsync<T>()
    {
        if (Closed)
        {
            return default;
        }

        await _handler.Semaphore.WaitAsync();

        foreach (var item in _handler.Output)
        {
            var result = item.TryGet<T>();
            if (result is not null)
            {
                _handler.Semaphore.Release();
                return result.Get();
            }
        }

        _handler.Semaphore.Release();
        return default;
    }

    public void SendInput<T>(T data)
    {
        if (Closed)
        {
            return;
        }

        var input = new ClientInput<T>()
        { 
            ClientId = _id,
            Data = data
        };
        _handler.InputQueue.Enqueue(input);
    }
}
