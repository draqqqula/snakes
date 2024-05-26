using FlatSharp;
using MessageSchemes;
using ServerEngine.Interfaces.Output;
using ServerEngine.Models;
using SnakeGame.Mechanics.ViewPort;
using SnakeGame.Models.Output.External;

namespace SnakeGame.Mechanics.ViewPort.Output;

internal class ViewPortBasedOutputTransformer(ViewPortManager Manager) : 
    IOutputCollector<ClientEventSet>, IOutputProvider<ViewPortBasedBinaryOutput>
{
    private readonly Dictionary<ClientIdentifier, EventMessage> _data = [];
    public ViewPortBasedBinaryOutput Get()
    {
        return new ViewPortBasedBinaryOutput(
            _data.ToDictionary(it => it.Key, it => Serialize(it.Value)));
    }

    public void Pass(ClientEventSet data)
    {
        _data.Add(data.Id, data.Table.SerializeFlatSharp());
    }

    private byte[] Serialize(EventMessage message)
    {
        var size = EventMessage.Serializer.GetMaxSize(message);
        var buffer = new byte[size + 4];
        var lenghtBytes = BitConverter.GetBytes(buffer.Length);
        lenghtBytes.CopyTo(buffer, 0);
        EventMessage.Serializer.Write(new SpanWriter(), buffer.AsSpan(4), message);
        return buffer;
    }
}
