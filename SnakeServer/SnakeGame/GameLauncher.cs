using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Services;
using ServerEngine.Models;
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
        services.AddSingleton<Dictionary<Guid, TilePickup>>();
        services.AddSingleton<Dictionary<ClientIdentifier, SnakeCharacter>>();

        services.AddSingleton<ICollisionResolver<Polygon, Polygon>, PolygonToPolygonResolver>();
        services.AddSingleton<ICollisionResolver<AxisAlignedBoundingBox, AxisAlignedBoundingBox>, AABBToAABBResolver>();
        services.AddSingleton<ICollisionResolver<RotatableSquare, RotatableSquare>, RSquareToRSquareResolver>();
        services.AddSingleton<ICollisionChecker, CollisionChecker>();

        services.AddSingleton<CharacterFabric, CharacterFabricA>();
        services.AddSingleton<PlayerSpawnerService>();
        services.AddSingleton<IInputService<MovementDirectionInput>>(provider => provider.GetRequiredService<PlayerSpawnerService>());
        services.AddSingleton<ISessionService>(provider => provider.GetRequiredService<PlayerSpawnerService>());
        services.AddSingleton<IOutputService<FrameDisplayOutput>>(provider => provider.GetRequiredService<PlayerSpawnerService>());

        services.AddSingleton<IUpdateService, SnakeBodyManager>();
        services.AddSingleton<IUpdateService, SnakePvPManager>();

        services.AddSingleton<PickupSpawnerService>();
        services.AddSingleton<IUpdateService>(provider => provider.GetRequiredService<PickupSpawnerService>());
        services.AddSingleton<IOutputService<FrameDisplayOutput>>(provider => provider.GetRequiredService<PickupSpawnerService>());

        services.AddSingleton<IInputFormatter<BinaryInput>, MovementDirectionInputFormatter>();
        services.AddOutputFabric<FrameDisplayOutput, BinaryOutput, FrameDisplayToBinaryOutputTransformer>();
    }
}
