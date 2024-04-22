namespace SyncStates;

public struct Monitored<T>
{
    internal readonly SyncPool Pool;
    internal readonly FieldAdress Adress;
    internal T _value;

    public T Value 
    { 
        get 
        { 
            return _value; 
        } 
        set
        {
            Pool.NofifyChanged(Adress);
            _value = value;
        }
    }

    public static explicit operator T(Monitored<T> obj) => obj.Value;
    public static implicit operator Monitored<T>(T value) => new() { _value = value };
}
