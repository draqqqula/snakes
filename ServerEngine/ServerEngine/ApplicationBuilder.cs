using ServerEngine.Interfaces;

namespace ServerEngine;

public class ApplicationBuilder
{
    public static IGameApplication BuildApplication()
    {
        return new GameApplication();
    }
}
