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
        await SendInLoopAsync<ViewPortBasedBinaryOutput>(connection, webSocket, it => it[id], cancellationTokenSource);
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

    private static async Task SendInLoopAsync<T>(ISessionConnection connection, WebSocket webSocket,
        Func<T, byte[]> func,
        CancellationTokenSource cts)
    {
        while (!webSocket.CloseStatus.HasValue)
        {
            var output = await connection.GetOutputAsync<T>();
            if (output is null)
            {
                continue;
            }
            try
            {
                var buffer = func(output);
                if (buffer.Length == 0)
                {
                    continue;
                }
                await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, buffer.Length),
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

    private static async Task SendInitialAsync<T>(ISessionConnection connection, WebSocket webSocket,
        Func<T, byte[]> func,
        CancellationTokenSource cts)
    {
        var output = await connection.GetOutputAsync<T>();
        if (output is null)
        {
            return;
        }
        try
        {
            var buffer = func(output);
            if (buffer.Length == 0)
            {
                return;
            }
            await webSocket.SendAsync(
            new ArraySegment<byte>(buffer, 0, buffer.Length),
            WebSocketMessageType.Binary,
            true,
            cts.Token);
        }
        catch (WebSocketException ex)
        {
            cts.Cancel();
            connection.Dispose();
        }
    }
}