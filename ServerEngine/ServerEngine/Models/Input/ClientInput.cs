using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;

namespace ServerEngine.Models.Input;

public abstract record ClientInput
{
    public required ClientIdentifier ClientId { get; init; }

    internal abstract void Broadcast(IServiceProvider services);
}

public record ClientInput<T> : ClientInput
{
    public required T Data { get; init; }

    internal override void Broadcast(IServiceProvider services)
    {
        var formatters = services.GetServices<IInputFormatter<T>>();
        foreach (var formatter in formatters)
        {
            if (formatter.TryResolve(Data, ClientId))
            {
                return;
            }
        }
    }
}
