namespace SubscriptionService.Models;

public class Subscription
{
    public int Id { get; set; }
    public required string SubscriptionId { get; set; }
    public int ServiceId { get; set; }
    public required string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
