using ServerEngine.Interfaces;
using ServerEngine.Models;

namespace ServerEngine;

internal class GameApplication : IGameApplication
{
    public async Task<ISessionManager> CreateSessionAsync(ISessionLauncher launcher)
    {
        var handler = new SessionHandler();
        var state = new GameState(handler);

        state.Initialize(launcher);
        var manager = new SessionManager(handler, state);

        return manager;
    }
}
