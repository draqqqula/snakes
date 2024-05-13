using ServerEngine.Models;

namespace SnakeGame.Models.Output.External;

internal class ViewPortBasedBinaryOutput
{
	private readonly Dictionary<ClientIdentifier, byte[]> _data = [];
	public object this[ClientIdentifier id]
	{
		get => _data[id];
	}
}
