using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MessageSchemes;
using FlatSharp;
using Microsoft.Xna.Framework;

namespace VisualApp;

internal class GameClient(WebSocket webSocket, GameApp game, FileStream log)
{
    private EventMessage LastMessage;
    volatile GameApp app = game;
    private volatile int Counter = 0;
    public async Task RecieveLoopAsync()
    {
        try
        {
            var counter = 0;
            while (!webSocket.CloseStatus.HasValue)
            {
                var buffer = new byte[10240];
                await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                var capacity = BitConverter.ToInt32(buffer.Take(8).ToArray());

                var message = EventMessage.Serializer.Parse(new MemoryInputBuffer(buffer.AsMemory(4, capacity)));
                counter++;
                Counter = counter;
                ApplyMessage(message);
            }
        }
        catch
        (Exception ex)
        {
            Console.WriteLine(LastMessage.ToString().Concat(app.DisplayBuffer.ToString()).Concat(Counter.ToString()));
        }
    }

    private void ApplyMessage(EventMessage message)
    {
        LastMessage = message;
        var created = message.Created?
                .Select(
                group => group.Frames.Select(it => FrameDisplayForm.FromMessage(it, group.Asset))
                )
                .SelectMany(it => it)
                .ToArray();
        foreach (var position in message.PositionEvents ?? [])
        {
            game.DisplayBuffer[position.Id].Sleeping = false;
            game.DisplayBuffer[position.Id].Position = new Vector2(position.Position.X, position.Position.Y);
        }
        foreach (var size in message.SizeEvents ?? [])
        {
            game.DisplayBuffer[size.Id].Sleeping = false;
            game.DisplayBuffer[size.Id].Scale = new Vector2(size.Size.X, size.Size.Y);
        }
        foreach (var angle in message.AngleEvents ?? [])
        {
            game.DisplayBuffer[angle.Id].Sleeping = false;
            game.DisplayBuffer[angle.Id].Rotation = angle.Angle;
        }
        foreach (var group in message.Transformations ?? [])
        {
            foreach (var id in group.Frames)
            {
                game.DisplayBuffer[id].Sleeping = false;
                game.DisplayBuffer[id].Name = group.NewAsset;
            }
        }
        foreach (var id in message.Disposed ?? [])
        {
            game.DisplayBuffer.Remove(id, out var value);
        }
        foreach (var id in message.Sleep ?? [])
        {
            game.DisplayBuffer[id].Sleeping = true;
        }
        foreach (var frame in created ?? [])
        {
            game.DisplayBuffer.TryAdd(frame.Id, frame);
        }
    }

    private const float DirectionMinDelta = MathF.PI/24;

    public async Task SendLoopAsync()
    {
        try
        {
            var currentAngle = 0f;
            while (!webSocket.CloseStatus.HasValue)
            {
                await game.Semaphore.WaitAsync();

                if (Math.Abs(currentAngle - game.Joystick.Direction) > DirectionMinDelta)
                {
                    await webSocket.SendAsync(
                    BitConverter.GetBytes(game.Joystick.Direction),
                    WebSocketMessageType.Binary,
                    true,
                    CancellationToken.None);

                    currentAngle = game.Joystick.Direction;
                }

                game.Semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            
        }
    }
}
