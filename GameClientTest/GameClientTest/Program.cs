using GameClientTest;
using System.Net.WebSockets;
using System.Numerics;
using System.Text.RegularExpressions;

var sessionId = Console.ReadLine();

var builder = new UriBuilder()
{
    Host = "localhost",
    Scheme = "wss",
    Port = 7170,
    Path = "sessions",
    Query = $"sessionId={sessionId}"
};
Console.WriteLine(builder.Uri);

using (ClientWebSocket ws = new ClientWebSocket())
{
    await ws.ConnectAsync(builder.Uri, CancellationToken.None);
    var client = new GameClient(ws);

    var sendTask = Task.Run(client.SendLoopAsync);
    await client.RecieveLoopAsync();
}