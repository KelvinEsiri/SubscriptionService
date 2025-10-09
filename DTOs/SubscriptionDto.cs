namespace SubscriptionService.DTOs;

public class SubscriptionRequest
{
    public required string service_id { get; set; }
    public required string token_id { get; set; }
    public required string phone_number { get; set; }
}

public class SubscriptionResponse
{
    public required string subscription_id { get; set; }
}
