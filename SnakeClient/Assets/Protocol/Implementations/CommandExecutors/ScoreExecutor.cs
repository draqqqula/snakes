using Assets.Protocol.Implementations.CommandExecutors;
using Assets.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

public class ScoreExecutor : ItemSetExecutor
{
    private ScoreController Controller;

    public override byte SignatureByte => 2;

    [Inject]
    public void Construct(ScoreController controller)
    {
        Controller = controller;
    }

    public override void ParseItem(Stream stream)
    {
        var team = (TeamColor)stream.ReadByte();
        var buffer = new byte[4];
        stream.Read(buffer, 0, 4);
        var score = BitConverter.ToInt32(buffer);
        Controller.ChangeValue(team, score);
    }
}
