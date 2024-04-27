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
        var session = storage.GetById(sessionId);
        if (HttpContext.WebSockets.IsWebSocketRequest && 
            session is not null && 
            session.GetStatus() == SessionStatus.Running)
        {
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
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        var id = new ClientIdentifier();
        var connection = await session.ConnectAsync(id);
        var recieveTask = Task.Run(() => RecieveInLoopAsync(connection, webSocket, cancellationTokenSource));
        await SendInLoopAsync(connection, webSocket, cancellationTokenSource);
    }

    private static async Task RecieveInLoopAsync(ISessionConnection connection, WebSocket webSocket,
        CancellationTokenSource cts)
    {
        var buffer = new byte[4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue && !connection.Closed)
        {
            await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            if (receiveResult.MessageType == WebSocketMessageType.Close)
            {
                connection.Dispose();
                return;
            }
            connection.SendInput(new BinaryInput() { Data = buffer });
        }
        if (!connection.Closed)
        {
            connection.Dispose();
        }
    }

    private static async Task SendInLoopAsync(ISessionConnection connection, WebSocket webSocket,
        CancellationTokenSource cts)
    {
        while (!webSocket.CloseStatus.HasValue)
        {
            var output = await connection.GetOutputAsync<BinaryOutput>();
            try
            {
                await webSocket.SendAsync(
                new ArraySegment<byte>(output.Data, 0, output.Data.Length),
                WebSocketMessageType.Binary,
                true,
                cts.Token);
            }
            catch (WebSocketException ex)
            {
                cts.Cancel();
                break;
            }
        }
        if (!connection.Closed)
        {
            connection.Dispose();
        }
    }
}