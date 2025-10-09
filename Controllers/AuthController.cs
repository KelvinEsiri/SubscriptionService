using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.DTOs;
using SubscriptionService.Models;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.service_id) || string.IsNullOrEmpty(request.password))
        {
            return BadRequest(new { message = "service_id and password are required" });
        }

        var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == request.service_id);

        if (service == null || service.Password != request.password)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var tokenId = Guid.NewGuid().ToString();
        var tokenValidityHours = _configuration.GetValue<int>("TokenValidityHours", 24);
        
        var token = new Token
        {
            TokenId = tokenId,
            ServiceId = service.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(tokenValidityHours)
        };

        _context.Tokens.Add(token);
        await _context.SaveChangesAsync();

        return Ok(new LoginResponse { token = tokenId });
    }
}
