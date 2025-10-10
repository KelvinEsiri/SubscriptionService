using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.DTOs;
using SubscriptionService.Models;

namespace SubscriptionService.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == request.service_id);

        if (service == null || service.Password != request.password)
        {
            return null;
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

        return new LoginResponse { token = tokenId };
    }
}
