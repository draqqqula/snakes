namespace ServerEngine.Interfaces;

public interface IGameContext
{
    public float DeltaTime { get; }
    public T Using<T>();
}
