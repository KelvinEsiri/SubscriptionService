using SubscriptionService.DTOs;
using SubscriptionService.Models;

namespace SubscriptionService.Services;

public interface IAuthService
{
    Task<ServiceResult<Service>> RegisterAsync(LoginRequest request);
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
}
