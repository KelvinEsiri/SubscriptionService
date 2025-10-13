namespace SubscriptionService.DTOs;

// Generic service result wrapper
// This is used to standardize responses from service methods
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }
}