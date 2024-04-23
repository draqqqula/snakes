using Microsoft.AspNetCore.Mvc;
using ServerEngine.Interfaces;
using ServerEngine.Models;
using SnakeGame.Models.Input.External;
using SnakeGame.Models.Output.External;
using System;
using System.Net.WebSockets;

namespace WebSocketsSample.Controllers;

[Route("sessions")]
public class SessionController : ControllerBase
{
    public async Task ConnectAsync(
        [FromQuery] Guid sessionId,
        [FromServices] ISessionStorage<Guid> storage)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var session = storage.GetById(sessionId);
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            await StartCommunicationAsync(webSocket, session);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task StartCommunicationAsync(WebSocket webSocket, ISessionManager session)
    {
        var id = new ClientIdentifier();
        var connection = await session.ConnectAsync(id);
        var recieveTask = Task.Run(() => RecieveInLoopAsync(connection, webSocket));
        await SendInLoopAsync(connection, webSocket);
    }

    private static async Task RecieveInLoopAsync(ISessionConnection connection, WebSocket webSocket)
    {
        var buffer = new byte[4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue && !connection.Closed)
        {
            await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            connection.SendInput(new BinaryInput() { Data = buffer });
        }
    }

    private static async Task SendInLoopAsync(ISessionConnection connection, WebSocket webSocket)
    {
        while (!webSocket.CloseStatus.HasValue)
        {
            var output = await connection.GetOutputAsync<BinaryOutput>();
            await webSocket.SendAsync(
                new ArraySegment<byte>(output.Data, 0, output.Data.Length),
                WebSocketMessageType.Binary,
                true,
                CancellationToken.None);
        }
    }
}