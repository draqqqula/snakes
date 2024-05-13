using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System;
using FlatBuffers;
using MessageSchemes;

public class WebSocketController : MonoBehaviour
{
    [field: SerializeField]
    public string ConnectionString { get; private set; }
    [field: SerializeField]
    public string SessionId { get; private set; }
    private WebSocket WebSocket { get; set; }
    public FrameDisplay FrameDisplay;
    public JoystickBehaviour JoyStick;
    private float CurrentAngle { get; set; }

    [field: SerializeField]
    public float DirectionDelta { get; set; }
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
        ByteBuffer loader = new ByteBuffer(buffer, 4);
        var message = Message.GetRootAsMessage(loader);
        FrameDisplay.Synchronize(message);
    }
}
