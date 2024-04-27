using ServerEngine.Models;

namespace SnakeGame.Models.Gameplay;

internal class TeamContext
{
    public uint Score { get; set; } = 0;
    public List<ClientIdentifier> Members { get; } = [];
}
