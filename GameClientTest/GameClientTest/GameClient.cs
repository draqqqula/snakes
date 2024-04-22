using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;

namespace GameClientTest;

internal class GameClient(WebSocket webSocket)
{

    public async Task RecieveLoopAsync()
    {
        while (!webSocket.CloseStatus.HasValue)
        {
            var buffer = new byte[4096];
            await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            var text = Encoding.UTF8.GetString(buffer);
            Console.WriteLine(text);
        }
    }

    public async Task SendLoopAsync()
    {
        while (!webSocket.CloseStatus.HasValue)
        {
            var key = Console.ReadKey();
            byte keyId = 0;
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow: keyId = 1; break;
                case ConsoleKey.RightArrow: keyId = 2; break;
                case ConsoleKey.DownArrow: keyId = 3; break;
                case ConsoleKey.UpArrow: keyId = 4; break;
            }
            Console.WriteLine($"pressed {key.Key}");
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
