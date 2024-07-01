using MessageSchemes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Common;
using SnakeGame.Configuration;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Resolvers;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Mechanics.Respawn;
using SnakeGame.Mechanics.ViewPort;
using SnakeGame.Mechanics.ViewPort.Display;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Input.External;
using SnakeGame.Models.Input.Internal;
using SnakeGame.Models.Output.External;
using SnakeGame.Models.Output.Internal;
using SnakeGame.Services;
using SnakeGame.Services.Gameplay;
using SnakeGame.Services.Gameplay.Abilities;
using SnakeGame.Services.Gameplay.Abilities.Dash;
using SnakeGame.Services.Gameplay.Abilities.Magnet;
using SnakeGame.Services.Gameplay.Abilities.Reverse;
using SnakeGame.Services.Gameplay.FrameDrivers;
using SnakeGame.Services.Gameplay.Spawners;
using SnakeGame.Services.Input;
using SnakeGame.Services.Output;
using SnakeGame.Services.Output.Services;
using SnakeGame.Systems.Movement;
using SnakeGame.Systems.Respawn;
using SnakeGame.Systems.Service;
using SnakeGame.Systems.Timer;
using SnakeGame.Systems.ViewPort.Interfaces;

namespace SnakeGame;

public class GameLauncher : ISessionLauncher
{
    public void Prepare(IServiceCollection services)
    {
        services.AddSingleton<ClientRegistry>();
        services.AddSingleton<IClientRegistry>(provider => provider.GetRequiredService<ClientRegistry>());
        services.AddSingleton<ISessionService>(provider => provider.GetRequiredService<ClientRegistry>());

        services.AddSingleton<TimerScheduler>();
        services.AddSingleton<ITimerScheduler>(provider => provider.GetRequiredService<TimerScheduler>());
        services.AddSingleton<IUpdateService>(provider => provider.GetRequiredService<TimerScheduler>());
        services.AddSingleton<IGameConfiguration, JsonGameConfiguration>();
        services.AddFrameProduction();

        services.AddSingleton<Dictionary<TeamColor, TeamContext>>();
        services.AddSingleton<List<PickupPoints>>();
        services.AddSingleton<Dictionary<TeamContext, List<Slime>>>();
        services.AddSingleton<Dictionary<ClientIdentifier, SnakeCharacter>>();

        services.AddSingleton<ICollisionResolver<Polygon, Polygon>, PolygonToPolygonResolver>();
        services.AddSingleton<ICollisionResolver<AABB, AABB>, AABBToAABBResolver>();
        services.AddSingleton<ICollisionResolver<RotatableSquare, RotatableSquare>, RSquareToRSquareResolver>();
        services.AddSingleton<ICollisionChecker, CollisionChecker>();

        services.AddSingleton<MinimapManager>();
        services.AddSingleton<ITrackingSource>(provider => provider.GetRequiredService<MinimapManager>());
        services.AddViewProduction();
        services.AddSingleton<IOutputService<ClientCommandWrapper>, ViewOutputService>();
        services.AddOutputFabricScoped<ClientCommandWrapper, ViewPortBasedBinaryOutput, ViewPortBasedOutputTransformer>();
        services.AddSingleton<CommandSender>();
        services.AddSingleton<IOutputService<ClientCommandWrapper>>(provider => provider.GetRequiredService<CommandSender>());
        services.AddViewHelp();

        services.AddSingleton(new MatchConfiguration()
        {
            Duration = TimeSpan.FromMinutes(10),
            ScoreToWin = 2048,
            TeamSize = 4,
            Mode = GameMode.Quad
        });
        services.AddSingleton<MatchManager>();
        services.AddSingleton<IUpdateService>(it => it.GetRequiredService<MatchManager>());
        services.AddSingleton<IStartUpService>(it => it.GetRequiredService<MatchManager>());
        services.AddSingleton<ISessionService>(it => it.GetRequiredService<MatchManager>());

        services.AddSingleton<ScoreManager>();
        services.AddSingleton<IStartUpService>(it => it.GetRequiredService<ScoreManager>());
        services.AddSingleton<ISessionService>(it => it.GetRequiredService<ScoreManager>());

        services.AddSingleton<SnakeSpawner>();
        services.AddSingleton<IInputService<MovementDirectionInput>>(provider => provider.GetRequiredService<SnakeSpawner>());
        services.AddSingleton<ISessionService>(provider => provider.GetRequiredService<SnakeSpawner>());
        services.AddSingleton<IUpdateService>(provider => provider.GetRequiredService<SnakeSpawner>());

        services.AddSingleton<IAbilityFactory, AbilityFactory<ReverseAbility>>();
        services.AddSingleton<IAbilityFactory, AbilityFactory<DashAbility>>();
        services.AddSingleton<IAbilityFactory, AbilityFactory<MagnetAbility>>();
        services.AddSingleton<RespawnManager>();
        services.AddSingleton<IInputService<OptionInput>>(provider => provider.GetRequiredService<RespawnManager>());
        services.AddSingleton<ISessionService>(provider => provider.GetRequiredService<RespawnManager>());
        services.AddSingleton<IUpdateService>(provider => provider.GetRequiredService<RespawnManager>());

        services.AddSingleton<IUpdateService, TrailMovementManager>();
        services.AddSingleton<IUpdateService, SnakeCollisionManager>();

        services.AddSingleton<PickupSpawner>();
        services.AddSingleton<IUpdateService>(provider => provider.GetRequiredService<PickupSpawner>());

        services.AddSingleton<SlimeSpawner>();
        services.AddSingleton<IUpdateService>(provider => provider.GetRequiredService<SlimeSpawner>());
        services.AddSingleton<IStartUpService>(provider => provider.GetRequiredService<SlimeSpawner>());

        services.AddSingleton<IInputService<AbilityActivationInput>, AbilityManager>();

        services.AddSingleton<IInputFormatter<BinaryInput>, MovementDirectionInputFormatter>();
        services.AddSingleton<IInputFormatter<BinaryInput>, AbilityInputFormatter>();
        services.AddSingleton<IInputFormatter<BinaryInput>, OptionInputFormatter>();
    }
}
