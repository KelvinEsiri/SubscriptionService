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
    public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
    {
        if (string.IsNullOrEmpty(request.service_id) || string.IsNullOrEmpty(request.password))
        {
            return BadRequest(new { message = "service_id and password are required" });
        }

        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] AuthRequest request)
    {
        if (string.IsNullOrEmpty(request.service_id) || string.IsNullOrEmpty(request.password))
        {
            return BadRequest(new { message = "service_id and password are required" });
        }

        var result = await _authService.RegisterAsync(request);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        }

        return StatusCode(result.StatusCode, new { message = "Service registered successfully", result.Data!.service_id });
    }
}
