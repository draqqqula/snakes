using GameTest.InputModels;
using GameTest.OutputModels;
using GameTest.Services;
using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Output;
using ServerEngine.Interfaces.Services;

namespace GameTest;

public class TestLauncher : ISessionLauncher
{
    public void Prepare(IServiceCollection services)
    {
        services.AddSingleton<TestUpdateService>();
        services.AddSingleton<IUpdateService>(provider => provider.GetRequiredService<TestUpdateService>());
        services.AddSingleton<ISessionService, TestClientService>();
        services.AddSingleton<IInputService<MovementInput>, TestClientService>();
        services.AddSingleton<IInputService<JumpInput>, JumpReader>();

        services.AddSingleton<IOutputService<TestOutput>>(provider => provider.GetRequiredService<TestUpdateService>());
        services.AddOutputFabric<TestOutput, BinaryOutput, CustomOutputTransformer>();
    }
}
