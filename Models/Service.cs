namespace SubscriptionService.Models;

public class Service
{
    public int Id { get; set; }
    public required string ServiceId { get; set; }
    public required string Password { get; set; }
}
