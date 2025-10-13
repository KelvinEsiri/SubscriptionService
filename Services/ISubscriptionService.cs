using SubscriptionService.DTOs;

namespace SubscriptionService.Services;

public interface ISubscriptionService
{
    Task<ServiceResult<SubscriptionResponse>> SubscribeAsync(SubscriptionRequest request);
    Task<ServiceResult<object>> UnsubscribeAsync(SubscriptionRequest request);
    Task<ServiceResult<StatusResponse>> GetStatusAsync(StatusRequest request);
}
