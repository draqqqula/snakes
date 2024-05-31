using Assets.State;
using Assets.State.Executors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FunctionInstaller : MonoInstaller
{
    public MinimapController MinimapController;
    public CameraBinder CameraBinder;
    public FrameDisplay Display;
    public WebSocketController WebSocketController;
    public TimerController TimerController;
    public ScoreController ScoreController;
    public override void InstallBindings()
    {
        Container.BindInstance(Display);
        Container.BindInstance(CameraBinder);
        Container.BindInstance(MinimapController);
        Container.BindInstance(TimerController);
        Container.BindInstance(ScoreController);
        Container.BindInterfacesAndSelfTo<EventExecutor>().AsSingle();
        Container.BindInterfacesAndSelfTo<AttachCameraExecutor>().AsSingle();
        Container.BindInterfacesAndSelfTo<MinimapExecutor>().AsSingle();
        Container.BindInterfacesAndSelfTo<TimerExecutor>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreExecutor>().AsSingle();
        Container.BindInterfacesAndSelfTo<CommandReader>().AsSingle();
        Container.BindInstance(WebSocketController);
    }
}
