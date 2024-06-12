using Microsoft.AspNetCore.Hosting;
using SessionApi.Filters;
using SnakeGame;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGameApplication();
builder.Services.AddGameLauncher();


var app = builder.Build();

// <snippet_UseWebSockets>
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseCors();
app.UseOptions();
app.UseWebSockets(webSocketOptions);
// </snippet_UseWebSockets>
//app.UseHttpsRedirection();

app.MapControllers();

app.Run();
