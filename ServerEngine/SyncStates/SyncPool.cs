using System.Collections.Frozen;
using System.Reflection;
using System.Threading;

namespace SyncStates;

public abstract class SyncPool
{
    internal abstract void NofifyChanged(FieldAdress adress);
}

public class SyncPool<T> : SyncPool
{
    internal override void NofifyChanged(FieldAdress adress)
    {

    }
    public void Register(T obj)
    {

    }

    private static FrozenDictionary<byte, FieldInfo> BuildFieldMap()
    {
        var monitoredFields = typeof(T)
        .GetFields()
            .Select((value, i) => (value, i))
            .ToFrozenDictionary(it => (byte)it.i, it => it.value);
        return monitoredFields;
    }
}
