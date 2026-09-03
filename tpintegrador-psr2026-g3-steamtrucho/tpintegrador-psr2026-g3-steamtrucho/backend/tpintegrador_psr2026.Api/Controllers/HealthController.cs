using Microsoft.AspNetCore.Mvc;

namespace tpintegrador_psr2026.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { estado = "ok" });
    }
}
