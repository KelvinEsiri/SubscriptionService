using Microsoft.AspNetCore.Mvc;
using SubscriptionService.DTOs;
using SubscriptionService.Services;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.service_id) || string.IsNullOrEmpty(request.password))
        {
            return BadRequest(new { message = "service_id and password are required" });
        }

        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        return Ok(result);
    }
}
