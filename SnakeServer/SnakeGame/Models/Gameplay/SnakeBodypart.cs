using SnakeCore.Extensions;
using SnakeGame.Common;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Systems.Movement;
using System.Numerics;

namespace SnakeGame.Models.Gameplay;

internal class SnakeBodypart : SquareBody
{
    private const float CatchDelta = 0.1f;

    public float DistanceCounter = 0f;
    public Queue<TrailSegment> Trail { get; set; } = [];
    public Trail Path { get; set; } = new Trail();
    public byte Tier { get; set; } = 0;
    public int Value => (int)Math.Pow(2, (Tier+1));
    private float Size { get; set; } = 4f;
    private float NormalSize { get; set; } = 4f;
    private float ReformFactor { get; set; } = 0.1f;
    public void Bounce()
    {
        Size = 6;
    }

    public void Update(float deltaTime)
    {
        if (Size > NormalSize || Transform.Size.X > NormalSize) 
        {
            if (Size <= NormalSize + CatchDelta)
            {
                Size = NormalSize;
                Transform.Size = new Vector2(Size, Size);
                return;
            }
            Size = MathEx.Lerp(Size, NormalSize, ReformFactor, deltaTime);
            Transform.Size = new Vector2(Size, Size);
        }
    }

    public void UpdateAsset(TeamColor team)
    {
        Transform.ChangeAsset($"body{Tier}_{team}");
    }
}
