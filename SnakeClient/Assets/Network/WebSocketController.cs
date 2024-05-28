using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System;
using FlatBuffers;
using MessageSchemes;
using Assets.State;
using Zenject;

public class WebSocketController : MonoBehaviour
{
    [field: SerializeField]
    public string ConnectionString { get; private set; }
    [field: SerializeField]
    public string SessionId { get; private set; }
    private WebSocket WebSocket { get; set; }

    public JoystickBehaviour JoyStick;
    private float CurrentAngle { get; set; }

    [field: SerializeField]
    public float DirectionDelta { get; set; }

    public IMessageReader Reader;

    [Inject]
    public void Construct(IMessageReader reader)
    {
        Reader = reader;
    }

    void Start()
    {
        WebSocket = new WebSocket(string.Concat(ConnectionString, SessionId));
        WebSocket.OnError += (err) => Debug.Log(err);
        WebSocket.OnMessage += OnMessage;
        WebSocket.Connect();
    }

    void Update()
    {
        if (WebSocket.State == WebSocketState.Open && Math.Abs(CurrentAngle - JoyStick.Direction) > DirectionDelta)
        {
            WebSocket.Send(BitConverter.GetBytes(JoyStick.Direction));
            CurrentAngle = JoyStick.Direction;
        }
        WebSocket.DispatchMessageQueue();
    }

    public void OnMessage(byte[] buffer)
    {
        Reader.Read(buffer);
    }
}
