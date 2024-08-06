using SnakeGame.Systems.Statistics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Systems.Statistics;

internal class RuntimeCommand<T>(IRuntimeCommandSender<T, ICompleted> Sender) : IRuntimeCommandSender<T, ICompleted>
{
    public ILink<T, ICompleted> Call()
    {
        return Sender.Call();
    }

    public static implicit operator RuntimeCommand<T>(RuntimeCommandSender<T, ICompleted> sender)
    {
        return new RuntimeCommand<T>(sender);
    }
}

internal class RuntimeCommand<T1, T2>(IRuntimeCommandSender<T1, ILink<T2, ICompleted>> Sender) : IRuntimeCommandSender<T1, ILink<T2, ICompleted>>
{
    public ILink<T1, ILink<T2, ICompleted>> Call()
    {
        return Sender.Call();
    }

    public static implicit operator RuntimeCommand<T1, T2>(RuntimeCommandSender<T1, ILink<T2,ICompleted>> sender)
    {
        return new RuntimeCommand<T1, T2>(sender);
    }
}

internal class RuntimeCommand<T1, T2, T3>(IRuntimeCommandSender<T1, ILink<T2, ILink<T3, ICompleted>>> Sender) : IRuntimeCommandSender<T1, ILink<T2, ILink<T3,ICompleted>>>
{
    public ILink<T1, ILink<T2, ILink<T3, ICompleted>>> Call()
    {
        return Sender.Call();
    }

    public static implicit operator RuntimeCommand<T1, T2, T3>(RuntimeCommandSender<T1, ILink<T2, ILink<T3, ICompleted>>> sender)
    {
        return new RuntimeCommand<T1, T2, T3>(sender);
    }
}
