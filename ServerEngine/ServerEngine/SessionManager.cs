using ServerEngine.Interfaces;
using ServerEngine.Models;
using ServerEngine.Models.Input;
using System.Diagnostics;
using System.Threading;

namespace ServerEngine;

internal class SessionManager : ISessionManager
{
    private const int FixedTimeStep = 16;
    private readonly Task _loopTask;
    private readonly SessionHandler _handler;

    public int ConnectedCount => _handler.PlayerCounter;

    public SessionManager(SessionHandler handler, GameState state)
    {
        _handler = handler;
        _loopTask = Task.Run(() => LoopUpdateAsync(state, handler).ContinueWith((task) => { OnClosed.Invoke(); }));
    }

    public event Action OnClosed = delegate { };

    public void Close()
    {
        _handler.Closed = true;
    }

    public async Task<ISessionConnection> ConnectAsync(ClientIdentifier id)
    {
        _handler.JoinQueue.Enqueue(id);
        _handler.PlayerCounter += 1;
        return new SessionConnection(_handler, id);
    }

    public SessionStatus GetStatus()
    {
        switch (_loopTask.Status)
        {
            case TaskStatus.Running: return SessionStatus.Running;
            case TaskStatus.Created: return SessionStatus.Pending;
            case TaskStatus.RanToCompletion: return SessionStatus.Completed;
            case TaskStatus.WaitingForActivation: return SessionStatus.Running;
            case TaskStatus.WaitingToRun: return SessionStatus.Pending;
            case TaskStatus.Faulted: return SessionStatus.Aborted;
            case TaskStatus.WaitingForChildrenToComplete: return SessionStatus.Completed;
            case TaskStatus.Canceled: return SessionStatus.Aborted;
            default: return SessionStatus.Unknown;
        }
    }
    private static async Task LoopUpdateAsync(GameState state, SessionHandler handler)
    {
        try
        {
            var stopWatch = Stopwatch.StartNew();
            while (!handler.Closed)
            {
                stopWatch.Restart();
                await handler.Semaphore.WaitAsync();
                state.Update(TimeSpan.FromMilliseconds(FixedTimeStep) * handler.TimeScale);

                while (stopWatch.ElapsedMilliseconds < FixedTimeStep)
                {

                }
                handler.Semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"Session closed due to unhandled exception \"{ex.Message}\"{ex.StackTrace}");
        }
    }
}