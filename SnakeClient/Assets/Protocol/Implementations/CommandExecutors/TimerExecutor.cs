using Assets.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Zenject;

public class TimerExecutor : ICommandExecutor
{
    private TimerController Controller;

    [Inject]
    public void Construct(TimerController timer)
    {
        Controller = timer;
    }

    public bool TryExecute(Stream stream)
    {
        if (stream.ReadByte() != 4)
        {
            stream.Position -= 1;
            return false;
        }
        var buffer = new byte[8];
        stream.Read(buffer, 0, 8);
        Controller.TimeElapsed = TimeSpan.FromSeconds(BitConverter.ToDouble(buffer));
        return true;
    }
}
