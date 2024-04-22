using GameTest.OutputModels;
using ServerEngine.Interfaces;
using ServerEngine.Interfaces.Output;
using ServerEngine.Interfaces.Services;

namespace GameTest.Services;

public class TestUpdateService : IUpdateService, IOutputService<TestOutput>
{
    public TestUpdateService()
    {
        Console.WriteLine($"Created Service {this.GetType().Name}");
    }

    private int Counter { get; set; }
    public void Update(IGameContext context)
    {
        Counter++;
        if (Counter % 1 == 0)
        {
            Console.WriteLine($"Update {Counter}");
        }
    }

    public void Write(IOutputCollector<TestOutput> writer)
    {
        writer.Pass(new TestOutput() 
        { 
            Text = $"Update {Counter}"
        });
    }
}
