namespace SubscriptionService.Models;

public class Token
{
    public int Id { get; set; }
    public required string TokenId { get; set; }
    public int ServiceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
