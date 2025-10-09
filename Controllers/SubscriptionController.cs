using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.DTOs;
using SubscriptionService.Models;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SubscriptionController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("subscribe")]
    public async Task<ActionResult<SubscriptionResponse>> Subscribe([FromBody] SubscriptionRequest request)
    {
        if (string.IsNullOrEmpty(request.service_id) || string.IsNullOrEmpty(request.token_id) || string.IsNullOrEmpty(request.phone_number))
        {
            return BadRequest(new { message = "service_id, token_id, and phone_number are required" });
        }

        var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == request.service_id);
        if (service == null)
        {
            return BadRequest(new { message = "Invalid service_id" });
        }

        var token = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenId == request.token_id && t.ServiceId == service.Id);
        if (token == null)
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        if (token.ExpiresAt < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Token expired" });
        }

        var existingSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.ServiceId == service.Id && s.PhoneNumber == request.phone_number && s.IsActive);

        if (existingSubscription != null)
        {
            return Conflict(new { message = "Already subscribed" });
        }

        var subscriptionId = Guid.NewGuid().ToString();
        var subscription = new Subscription
        {
            SubscriptionId = subscriptionId,
            ServiceId = service.Id,
            PhoneNumber = request.phone_number,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return Ok(new SubscriptionResponse { subscription_id = subscriptionId });
    }

    [HttpPost("unsubscribe")]
    public async Task<ActionResult> Unsubscribe([FromBody] SubscriptionRequest request)
    {
        if (string.IsNullOrEmpty(request.service_id) || string.IsNullOrEmpty(request.token_id) || string.IsNullOrEmpty(request.phone_number))
        {
            return BadRequest(new { message = "service_id, token_id, and phone_number are required" });
        }

        var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == request.service_id);
        if (service == null)
        {
            return BadRequest(new { message = "Invalid service_id" });
        }

        var token = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenId == request.token_id && t.ServiceId == service.Id);
        if (token == null)
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        if (token.ExpiresAt < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Token expired" });
        }

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.ServiceId == service.Id && s.PhoneNumber == request.phone_number && s.IsActive);

        if (subscription == null)
        {
            return NotFound(new { message = "No active subscription found" });
        }

        subscription.IsActive = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Unsubscribed successfully" });
    }

    [HttpPost("status")]
    public async Task<ActionResult<StatusResponse>> GetStatus([FromBody] StatusRequest request)
    {
        if (string.IsNullOrEmpty(request.service_id) || string.IsNullOrEmpty(request.token_id) || string.IsNullOrEmpty(request.phone_number))
        {
            return BadRequest(new { message = "service_id, token_id, and phone_number are required" });
        }

        var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == request.service_id);
        if (service == null)
        {
            return BadRequest(new { message = "Invalid service_id" });
        }

        var token = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenId == request.token_id && t.ServiceId == service.Id);
        if (token == null)
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        if (token.ExpiresAt < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Token expired" });
        }

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.ServiceId == service.Id && s.PhoneNumber == request.phone_number && s.IsActive);

        if (subscription == null)
        {
            return Ok(new StatusResponse 
            { 
                status = "not_subscribed",
                subscription_date = null
            });
        }

        return Ok(new StatusResponse 
        { 
            status = "subscribed",
            subscription_date = subscription.CreatedAt
        });
    }
}
