using Assets.State;
using Assets.State.Executors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FunctionInstaller : MonoInstaller
{
    public CameraBinder CameraBinder;
    public FrameDisplay Display;
    public WebSocketController Controller;
    public override void InstallBindings()
    {
        Container.BindInstance(Display);
        Container.BindInstance(CameraBinder);
        Container.BindInterfacesAndSelfTo<EventExecutor>().AsSingle();
        Container.BindInterfacesAndSelfTo<CommandReader>().AsSingle();
        Container.BindInterfacesAndSelfTo<AttachCameraExecutor>().AsSingle();
        Container.BindInstance(Controller);
    }
}
