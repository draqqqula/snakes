using SnakeGame.Mechanics.Frames;

namespace SnakeGame.Mechanics.ViewPort;

internal class ViewFrameRepository : LegacyFrameRepository
{
    public ViewFrameRepository(FrameRegistry Registry, FrameStorage Storage) : base(Registry, Storage)
    {
    }
}
