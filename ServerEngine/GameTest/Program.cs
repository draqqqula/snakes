using GameTest;
using GameTest.InputModels;
using GameTest.OutputModels;
using ServerEngine;
using ServerEngine.Interfaces;
using ServerEngine.Models;

// Создаём приложение
var app = ApplicationBuilder.BuildApplication();

// Создаём лаунчер
var launcher = new TestLauncher();

// Создаём сессии
var controller1 = await app.CreateSessionAsync(launcher);

await Task.Delay(500);

// Подключаем клиента
var client1 = new ClientIdentifier();
var client2 = new ClientIdentifier();

var connection1 = await controller1.ConnectAsync(client1);
var connection2 = await controller1.ConnectAsync(client2);

var recieveTask1 = Task.Run(() => RecieveData(connection1));
var recieveTask2 = Task.Run(() => RecieveData(connection2));

await Task.Delay(1000);

// Отправляем инпут от клиента
var distance = 15;
var input = new MovementInput()
{ 
    Distance = distance 
};
connection1.SendInput(input);

// Отправляем инпут от клиента
var height = 20;
var inputJump = new JumpInput()
{
    Height = height
};
connection2.SendInput(inputJump);

await Task.Delay(1000);

// Отключаем клиента
connection1.Dispose();

await Task.Delay(1000);

static async Task RecieveData(ISessionConnection connection)
{
    while (!connection.Closed)
    {
        var reader = await connection.GetOutputAsync<BinaryOutput>();
        var output = new BinaryOutput() { Data = new byte[1] };
        Console.WriteLine(output.Data.Length);
    }
}