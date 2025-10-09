namespace SubscriptionService.DTOs;

public class StatusRequest
{
    public required string service_id { get; set; }
    public required string token_id { get; set; }
    public required string phone_number { get; set; }
}

public class StatusResponse
{
    public required string status { get; set; }
    public DateTime? subscription_date { get; set; }
}
