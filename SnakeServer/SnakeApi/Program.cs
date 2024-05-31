using SnakeGame;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGameApplication();
builder.Services.AddGameLauncher();
builder.Services.AddCors();

var app = builder.Build();

// <snippet_UseWebSockets>
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);
// </snippet_UseWebSockets>
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();

app.MapControllers();

app.Run();
