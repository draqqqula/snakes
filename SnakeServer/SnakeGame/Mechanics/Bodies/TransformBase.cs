using System.Numerics;

namespace SnakeGame.Mechanics.Bodies
{
    internal abstract class TransformBase : IDisposable
    {
        public virtual Vector2 Position { get; set; }
        public virtual float Angle { get; set; }
        public virtual Vector2 Size { get; set; }

        public Transform ReadOnly => new Transform()
        {
            Angle = Angle,
            Position = Position,
            Size = Size,
        };

        public abstract void Dispose();

        public abstract void ChangeAsset(string newAsset);
    }
}
