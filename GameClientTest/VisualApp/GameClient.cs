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

namespace VisualApp;

internal class GameClient(WebSocket webSocket, GameApp game, FileStream log)
{

    public async Task RecieveLoopAsync()
    {
        var counter = 0;
        while (!webSocket.CloseStatus.HasValue)
        {
            var buffer = new byte[409600];
            await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            var options = new JsonSerializerOptions()
            {
                IncludeFields = true,
            };
            var stream = new MemoryStream();
            var capacity = BitConverter.ToInt32(buffer.Take(8).ToArray());
            //log.Write(Encoding.UTF8.GetBytes($"\n{counter}"));
            //log.Write(buffer, 8, capacity);
            stream.Write(buffer, 8, capacity);
            stream.Position = 0;
            var pool = await JsonSerializer.DeserializeAsync<FrameDisplayForm[]>(stream, options);
            game.DisplayBuffer = pool;
            counter++;
        }
    }

    private const float DirectionMinDelta = MathF.PI/24;

    public async Task SendLoopAsync()
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
}
