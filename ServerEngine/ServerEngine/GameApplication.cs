using ServerEngine.Interfaces;

namespace ServerEngine;

internal class GameApplication : IGameApplication
{
    private const int FixedTimeStep = 20;

    public async Task<ISessionManager> CreateSessionAsync(ISessionLauncher launcher)
    {
        var handler = new SessionHandler();
        var state = new GameState(handler);

        state.Initialize(launcher);
        var loop = Task.Run(() => LoopUpdateAsync(state, handler));
        var manager = new SessionManager(handler);

        return manager;
    }

    private static async Task LoopUpdateAsync(GameState state, SessionHandler handler)
    {
        while (!handler.Closed)
        {
            await handler.Semaphore.WaitAsync();
            await Task.Delay(FixedTimeStep);

            state.Update(TimeSpan.FromMilliseconds(FixedTimeStep));
            handler.Semaphore.Release();
        }
    }
}
