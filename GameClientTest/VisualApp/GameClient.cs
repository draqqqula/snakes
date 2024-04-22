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

internal class GameClient(WebSocket webSocket, GameApp game)
{

    public async Task RecieveLoopAsync()
    {
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
            stream.Write(buffer, 8, capacity);
            stream.Position = 0;
            var pool = await JsonSerializer.DeserializeAsync<FrameDisplayForm[]>(stream, options);
            game.DisplayBuffer = pool;
        }
    }

    public async Task SendLoopAsync()
    {
        while (!webSocket.CloseStatus.HasValue)
        {
            while (game.PressedKeys.TryDequeue(out var key))
            {
                byte keyId = 0;
                switch (key)
                {
                    case Keys.Left: keyId = 1; break;
                    case Keys.Right: keyId = 2; break;
                    case Keys.Down: keyId = 3; break;
                    case Keys.Up: keyId = 4; break;
                }
                if (keyId != 0)
                {
                    await webSocket.SendAsync(
                    new byte[1] { keyId },
                    WebSocketMessageType.Binary,
                    true,
                    CancellationToken.None);
                }
            }
        }
    }
}
