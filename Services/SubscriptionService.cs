using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.DTOs;
using SubscriptionService.Models;

namespace SubscriptionService.Services;

public class SubscriptionManagementService : ISubscriptionService
{
    private readonly ApplicationDbContext _context;

    public SubscriptionManagementService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<SubscriptionResponse>> SubscribeAsync(SubscriptionRequest request)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == request.service_id);
        if (service == null)
        {
            return new ServiceResult<SubscriptionResponse>
            {
                Success = false,
                ErrorMessage = "Invalid service_id",
                StatusCode = 400
            };
        }

        var token = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenId == request.token_id && t.ServiceId == service.Id);
        if (token == null)
        {
            return new ServiceResult<SubscriptionResponse>
            {
                Success = false,
                ErrorMessage = "Invalid token",
                StatusCode = 401
            };
        }

        if (token.ExpiresAt < DateTime.UtcNow)
        {
            return new ServiceResult<SubscriptionResponse>
            {
                Success = false,
                ErrorMessage = "Token expired",
                StatusCode = 401
            };
        }

        var existingSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.ServiceId == service.Id && s.PhoneNumber == request.phone_number && s.IsActive);

        if (existingSubscription != null)
        {
            return new ServiceResult<SubscriptionResponse>
            {
                Success = false,
                ErrorMessage = "Already subscribed",
                StatusCode = 409
            };
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

        return new ServiceResult<SubscriptionResponse>
        {
            Success = true,
            Data = new SubscriptionResponse { subscription_id = subscriptionId },
            StatusCode = 200
        };
    }

    public async Task<ServiceResult<object>> UnsubscribeAsync(SubscriptionRequest request)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == request.service_id);
        if (service == null)
        {
            return new ServiceResult<object>
            {
                Success = false,
                ErrorMessage = "Invalid service_id",
                StatusCode = 400
            };
        }

        var token = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenId == request.token_id && t.ServiceId == service.Id);
        if (token == null)
        {
            return new ServiceResult<object>
            {
                Success = false,
                ErrorMessage = "Invalid token",
                StatusCode = 401
            };
        }

        if (token.ExpiresAt < DateTime.UtcNow)
        {
            return new ServiceResult<object>
            {
                Success = false,
                ErrorMessage = "Token expired",
                StatusCode = 401
            };
        }

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.ServiceId == service.Id && s.PhoneNumber == request.phone_number && s.IsActive);

        if (subscription == null)
        {
            return new ServiceResult<object>
            {
                Success = false,
                ErrorMessage = "No active subscription found",
                StatusCode = 404
            };
        }

        subscription.IsActive = false;
        await _context.SaveChangesAsync();

        return new ServiceResult<object>
        {
            Success = true,
            Data = new { message = "Unsubscribed successfully" },
            StatusCode = 200
        };
    }

    public async Task<ServiceResult<StatusResponse>> GetStatusAsync(StatusRequest request)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == request.service_id);
        if (service == null)
        {
            return new ServiceResult<StatusResponse>
            {
                Success = false,
                ErrorMessage = "Invalid service_id",
                StatusCode = 400
            };
        }

        var token = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenId == request.token_id && t.ServiceId == service.Id);
        if (token == null)
        {
            return new ServiceResult<StatusResponse>
            {
                Success = false,
                ErrorMessage = "Invalid token",
                StatusCode = 401
            };
        }

        if (token.ExpiresAt < DateTime.UtcNow)
        {
            return new ServiceResult<StatusResponse>
            {
                Success = false,
                ErrorMessage = "Token expired",
                StatusCode = 401
            };
        }

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.ServiceId == service.Id && s.PhoneNumber == request.phone_number && s.IsActive);

        if (subscription == null)
        {
            return new ServiceResult<StatusResponse>
            {
                Success = true,
                Data = new StatusResponse 
                { 
                    status = "not_subscribed",
                    subscription_date = null
                },
                StatusCode = 200
            };
        }

        return new ServiceResult<StatusResponse>
        {
            Success = true,
            Data = new StatusResponse 
            { 
                status = "subscribed",
                subscription_date = subscription.CreatedAt
            },
            StatusCode = 200
        };
    }
}
