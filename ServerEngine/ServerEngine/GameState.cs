using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Output;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using ServerEngine.Models.Input;

namespace ServerEngine;

internal class GameState(SessionHandler handler) : IGameState
{
    private readonly SessionHandler _handler = handler;
    private readonly IServiceCollection _services = new ServiceCollection();

    private GameContext Context { get; set; }
    private IServiceProvider Provider { get; set; }
    private IEnumerable<IUpdateService> UpdateServices { get; set; } = [];
    private IEnumerable<ISessionService> SessionServices { get; set; } = [];
    private IEnumerable<OutputHandler> OutputManagers { get; set; } = [];

    public void Initialize(ISessionLauncher launcher)
    {
        _services.AddSingleton<IInternalSessionController>(new InternalSessionController(_handler));
        launcher.Prepare(this._services);

        Provider = _services.BuildServiceProvider();
        var provider = Provider;

        Context = new GameContext(Provider);

        var services = provider.GetServices<IStartUpService>();
        foreach (var service in services)
        {
            service.Start(Context);
        }
        UpdateServices = provider.GetServices<IUpdateService>();
        SessionServices = provider.GetServices<ISessionService>();
    }

    public void Update(TimeSpan deltaTime)
    {
        using IServiceScope updateScope = Provider.CreateScope();
        {
            ManageInput();

            ManageJoinQueue();

            ManageUpdateServices(updateScope, deltaTime);

            ManageLeaveQueue();

            ManageOutput(updateScope);

            _handler.TickCounter++;
        }
    }

    private void ManageUpdateServices(IServiceScope scope, TimeSpan deltaTime)
    {
        Context.DeltaTime = Convert.ToSingle(deltaTime.TotalSeconds);
        foreach (var service in scope.ServiceProvider.GetServices<IUpdateService>())
        {
            service.Update(Context);
        }
    }

    private void ManageLeaveQueue()
    {
        while (_handler.LeaveQueue.TryDequeue(out ClientIdentifier id))
        {
            foreach (var service in SessionServices)
            {
                service.OnLeave(Context, id);
            }
        }
    }

    private void ManageJoinQueue()
    {
        while (_handler.JoinQueue.TryDequeue(out ClientIdentifier id))
        {
            foreach (var service in SessionServices)
            {
                service.OnJoin(Context, id);
            }
        }
    }

    private void ManageInput()
    {
        while (_handler.InputQueue.TryDequeue(out ClientInput input))
        {
            input.Broadcast(Provider);
        }
    }

    private void ManageOutput(IServiceScope scope)
    {
        _handler.Output.Clear();
        var services = scope.ServiceProvider.GetServices<OutputHandler>();
        foreach (var manager in services)
        {
            _handler.Output.Add(manager.CreateMessage());
        }
    }
}
