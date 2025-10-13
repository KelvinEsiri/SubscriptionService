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

    // Register a new service
    public async Task<ServiceResult<AuthRequest>> RegisterAsync(AuthRequest request)
    {
        if (await _context.Services.AnyAsync(s => s.ServiceId == request.service_id))
        {
            return new ServiceResult<AuthRequest>
            {
                Success = false,
                ErrorMessage = "Service ID already exists",
                StatusCode = 409
            };
        }

        var service = new Service
        {
            ServiceId = request.service_id,
            Password = request.password
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return new ServiceResult<AuthRequest>
        {
            Success = true,
            Data = request,
            StatusCode = 201
        };
    }

    // Authenticate service and generate token
    public async Task<ServiceResult<AuthResponse>> LoginAsync(AuthRequest request)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == request.service_id);

        if (service == null)
        {
            return new ServiceResult<AuthResponse>
            {
                Success = false,
                ErrorMessage = "Invalid service_id",
                StatusCode = 400
            };
        }

        if (service.Password != request.password)
        {
            return new ServiceResult<AuthResponse>
            {
                Success = false,
                ErrorMessage = "Invalid password",
                StatusCode = 401
            };
        }

        // Check for existing valid token
        var existingToken = await _context.Tokens
            .Where(t => t.ServiceId == service.Id && t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.ExpiresAt)
            .FirstOrDefaultAsync();

        if (existingToken != null)
        {
            return new ServiceResult<AuthResponse>
            {
                Success = true,
                Data = new AuthResponse { token = existingToken.TokenId },
                StatusCode = 200
            };
        }

        // Generate new token using Guid which is a .NET built-in method to generate unique identifiers
        var tokenId = Guid.NewGuid().ToString();
        // Token validity duration from configuration (default to 24 hours if not set)
        var tokenValidityHours = _configuration.GetValue("TokenValidityHours", 24);

        var token = new Token
        {
            TokenId = tokenId,
            ServiceId = service.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(tokenValidityHours)
        };

        _context.Tokens.Add(token);
        await _context.SaveChangesAsync();

        return new ServiceResult<AuthResponse>
        {
            Success = true,
            Data = new AuthResponse { token = tokenId },
            StatusCode = 200
        };
    }
}
