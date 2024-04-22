using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Interfaces.Output;

namespace ServerEngine.Interfaces;

public interface ISessionLauncher
{
    public void Prepare(IServiceCollection services);
}

public static class LauncherExtensions
{
    public static void AddOutputHandler<TCollect,TProvide>(this IServiceCollection services)
    {
        services.AddScoped<OutputHandler, OutputHandler<TCollect,TProvide>>();
    }

    public static void AddOutputFabric<TCollect, TProvide, TTransformer>(this IServiceCollection services) where TTransformer : class, IOutputCollector<TCollect>, IOutputProvider<TProvide>
    {
        services.AddScoped<TTransformer>();
        services.AddScoped<IOutputProvider<TProvide>>(provider => provider.GetRequiredService<TTransformer>());
        services.AddScoped<IOutputCollector<TCollect>>(provider => provider.GetRequiredService<TTransformer>());
        services.AddOutputHandler<TCollect, TProvide>();
    }
}