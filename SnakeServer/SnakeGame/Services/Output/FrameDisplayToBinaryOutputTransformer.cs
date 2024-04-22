using MessagePack;
using ServerEngine.Interfaces.Output;
using SnakeGame.Models.Output.External;
using SnakeGame.Models.Output.Internal;
using System.Text;
using System.Text.Json;

namespace SnakeGame.Services.Output;

internal class FrameDisplayToBinaryOutputTransformer : IOutputCollector<FrameDisplayOutput>, IOutputProvider<BinaryOutput>
{
    private List<FrameDisplayOutput> Frames { get; } = [];
    public BinaryOutput Get()
    {
        return new BinaryOutput()
        {
            Data = BuildMessage()
        };
    }

    public void Pass(FrameDisplayOutput data)
    {
        Frames.Add(data);
    }

    private byte[] BuildMessage()
    {
        var options = new JsonSerializerOptions()
        {
            IncludeFields = true,
        };
        var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, Frames.ToArray(), options);
        var list = new List<byte>();
        var lenghtBytes = BitConverter.GetBytes(stream.Length);
        list.AddRange(lenghtBytes);
        list.AddRange(stream.ToArray());
        return list.ToArray();
    }
}
