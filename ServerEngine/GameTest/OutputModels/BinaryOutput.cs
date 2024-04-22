namespace GameTest.OutputModels;

public record BinaryOutput
{
    public required byte[] Data { get; init; }
}
