using ServerEngine.Models;

namespace SnakeGame.Models.Gameplay;

internal class TeamContext(TeamArea area)
{
    public uint Score { get; set; } = 0;
    public List<ClientIdentifier> Members { get; } = [];
    public TeamArea Area { get; set; } = area;
}
