using ServerEngine.Interfaces.Output;

namespace GameTest.OutputModels;

public class CustomOutputTransformer : IOutputCollector<TestOutput>, IOutputProvider<BinaryOutput>
{
    private readonly List<TestOutput> Collection = [];
    public BinaryOutput Get()
    {
        return new BinaryOutput { Data = new byte[Collection.Count] };
    }

    public void Pass(TestOutput data)
    {
        Collection.Add(data);
    }
}
