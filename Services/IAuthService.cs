using SubscriptionService.DTOs;

namespace SubscriptionService.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
