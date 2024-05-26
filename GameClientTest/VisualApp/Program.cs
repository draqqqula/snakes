
using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using VisualApp;
using System.Net;

var sessionId = File.ReadLines("sessionId.txt").First();
var builder = new UriBuilder()
{
    Host = "localhost",
    Scheme = "wss",
    Port = 7170,
    Path = "sessions",
    Query = $"sessionId={sessionId}"
};
using var game = new VisualApp.GameApp();
using (ClientWebSocket ws = new ClientWebSocket())
{
    ws.Options.RemoteCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
    await ws.ConnectAsync(builder.Uri, CancellationToken.None);
    var client = new GameClient(ws, game);

    var sendTask = Task.Run(client.SendLoopAsync);
    var recieveTask = Task.Run(client.RecieveLoopAsync);
    game.Run();
}