using ServerEngine.Profiling.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerEngine.Profiling;

internal class ProfilingManager : IProfileRecorder, IProfileAggregator
{
    private readonly Dictionary<object, TimeSpan> _recordings = [];
    private readonly Dictionary<object, TimeSpan> _actual = [];
    private readonly Dictionary<object, DateTime> _active = [];

    public string CreateSnapshot()
    {
        throw new NotImplementedException();
    }

    public TimeSpan GetTotalExecutionTime(object target)
    {
        return _recordings[target];
    }

    public void Start(object target)
    {
        _active.Add(target, DateTime.Now);
    }

    public void Stop(object target)
    {
        if (_active.TryGetValue(target, out var start))
        {
            var duration = DateTime.Now - start;
            if (_recordings.TryGetValue(target, out var total))
            {
                _recordings[target] = total + duration;
            }
            else
            {
                _recordings.Add(target, duration);
            }
            _actual[target] = duration;
            _active.Remove(target);
        }
    }
}
