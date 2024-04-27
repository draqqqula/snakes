using Microsoft.AspNetCore.Mvc;
using ServerEngine.Interfaces;

namespace SessionApi.Controllers;

public class LaunchController : ControllerBase
{
    [Route("launch")]
    public async Task<IActionResult> LaunchAsync(
        [FromServices] IGameApplication game, 
        [FromServices] ISessionLauncher launcher,
        [FromServices] ISessionStorage<Guid> storage)
    {
        var session = await game.CreateSessionAsync(launcher);
        var id = storage.Add(session);
        session.OnClosed += () => storage.Remove(id);
        return Ok(id);
    }
}
