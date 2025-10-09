namespace SubscriptionService.DTOs;

public class LoginRequest
{
    public required string service_id { get; set; }
    public required string password { get; set; }
}

public class LoginResponse
{
    public required string token { get; set; }
}
