using UnityEngine;
using NativeWebSocket;
using System;
using Assets.State;
using Zenject;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public class WebSocketController : MonoBehaviour
{
    [SerializeField]
    public bool AutoCreateSession;

    [field: SerializeField]
    public string ConnectionString { get; private set; }
    [field: SerializeField]
    public string LaunchString { get; private set; }
    [field: SerializeField]
    public string SessionId { get; private set; }
    private WebSocket WebSocket { get; set; }

    public JoystickBehaviour JoyStick;
    private float CurrentAngle { get; set; }

    [field: SerializeField]
    public float DirectionDelta { get; set; }

    public UnityWebRequest CreateSessionRequest;

    public IMessageReader Reader;

    private bool SessionFound = false;

    [Inject]
    public void Construct(IMessageReader reader)
    {
        Reader = reader;
    }

    async void Start()
    {
        if (AutoCreateSession)
        {
            CreateSessionRequest = UnityWebRequest.Get(LaunchString);
            CreateSessionRequest.SendWebRequest().completed += async _ => 
            {
                var regex = new Regex("[^\"]+");
                SessionId = regex.Match(CreateSessionRequest.downloadHandler.text).Value;
                await EstablishConnectionAsync();
            };
            return;
        }
        await EstablishConnectionAsync();
    }

    private async Task EstablishConnectionAsync()
    {
        SessionFound = true;
        WebSocket = new WebSocket(string.Concat(ConnectionString, SessionId));
        WebSocket.OnError += (err) => Debug.Log(err);
        WebSocket.OnMessage += OnMessage;
        await WebSocket.Connect();
    }

    async void Update()
    {
        if (!SessionFound)
        {
            return;
        }
        if (WebSocket.State == WebSocketState.Open && Math.Abs(CurrentAngle - JoyStick.Direction) > DirectionDelta)
        {
            await WebSocket.Send(BitConverter.GetBytes(JoyStick.Direction));
            CurrentAngle = JoyStick.Direction;
        }
        #if !UNITY_WEBGL || UNITY_EDITOR
            WebSocket.DispatchMessageQueue();
        #endif
    }

    public void OnMessage(byte[] buffer)
    {
        Reader.Read(buffer);
    }
}