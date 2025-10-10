using SubscriptionService.DTOs;

namespace SubscriptionService.Services;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }
}

public interface ISubscriptionService
{
    Task<ServiceResult<SubscriptionResponse>> SubscribeAsync(SubscriptionRequest request);
    Task<ServiceResult<object>> UnsubscribeAsync(SubscriptionRequest request);
    Task<ServiceResult<StatusResponse>> GetStatusAsync(StatusRequest request);
}
