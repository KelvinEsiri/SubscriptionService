namespace SubscriptionService.DTOs;

public class AuthRequest
{
    public required string service_id { get; set; }
    public required string password { get; set; }
}

public class AuthResponse
{
    public required string token { get; set; }
}
