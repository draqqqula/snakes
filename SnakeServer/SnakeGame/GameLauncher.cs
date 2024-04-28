using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
using SnakeGame.Common;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Resolvers;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Models.Gameplay;
using SnakeGame.Models.Input.External;
using SnakeGame.Models.Input.Internal;
using SnakeGame.Models.Output.External;
using SnakeGame.Models.Output.Internal;
using SnakeGame.Services.Gameplay;
using SnakeGame.Services.Input;
using SnakeGame.Services.Output;

namespace SnakeGame;

public class GameLauncher : ISessionLauncher
{
    public void Prepare(IServiceCollection services)
    {
        services.AddSingleton<Dictionary<TeamColor, TeamContext>>();
        services.AddSingleton<Dictionary<Guid, TilePickup>>();
        services.AddSingleton<Dictionary<ClientIdentifier, SnakeCharacter>>();

        services.AddSingleton<ICollisionResolver<Polygon, Polygon>, PolygonToPolygonResolver>();
        services.AddSingleton<ICollisionResolver<AxisAlignedBoundingBox, AxisAlignedBoundingBox>, AABBToAABBResolver>();
        services.AddSingleton<ICollisionResolver<RotatableSquare, RotatableSquare>, RSquareToRSquareResolver>();
        services.AddSingleton<ICollisionChecker, CollisionChecker>();

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

        services.AddSingleton<AreaManager>();
        services.AddSingleton<IOutputService<FrameDisplayOutput>>(it => it.GetRequiredService<AreaManager>());
        services.AddSingleton<IUpdateService>(it => it.GetRequiredService<AreaManager>());

        services.AddSingleton<CharacterFabric, CharacterFabricA>();
        services.AddSingleton<SnakeSpawner>();
        services.AddSingleton<IInputService<MovementDirectionInput>>(provider => provider.GetRequiredService<SnakeSpawner>());
        services.AddSingleton<ISessionService>(provider => provider.GetRequiredService<SnakeSpawner>());
        services.AddSingleton<IOutputService<FrameDisplayOutput>>(provider => provider.GetRequiredService<SnakeSpawner>());

        services.AddSingleton<IUpdateService, SnakeMovementManager>();
        services.AddSingleton<IUpdateService, SnakeCollisionManager>();

        services.AddSingleton<PickupSpawner>();
        services.AddSingleton<IUpdateService>(provider => provider.GetRequiredService<PickupSpawner>());
        services.AddSingleton<IOutputService<FrameDisplayOutput>>(provider => provider.GetRequiredService<PickupSpawner>());

        services.AddSingleton<IInputFormatter<BinaryInput>, MovementDirectionInputFormatter>();
        services.AddOutputFabric<FrameDisplayOutput, BinaryOutput, FrameDisplayToBinaryOutputTransformer>();
    }
}
