using SubscriptionService.DTOs;

namespace SubscriptionService.Services;

public interface IAuthService
{
    Task<ServiceResult<AuthRequest>> RegisterAsync(AuthRequest request);
    Task<ServiceResult<AuthResponse>> LoginAsync(AuthRequest request);
}
