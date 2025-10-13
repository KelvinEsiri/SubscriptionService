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

    // Validate service and token
    private async Task<ServiceResult<Service>> ValidateServiceAndTokenAsync(string serviceId, string tokenId)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceId);
        if (service == null)
        {
            return new ServiceResult<Service>
            {
                Success = false,
                ErrorMessage = "Invalid service_id",
                StatusCode = 400
            };
        }

        var token = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenId == tokenId && t.ServiceId == service.Id);
        if (token == null)
        {
            return new ServiceResult<Service>
            {
                Success = false,
                ErrorMessage = "Invalid token",
                StatusCode = 401
            };
        }

        if (token.ExpiresAt < DateTime.UtcNow)
        {
            return new ServiceResult<Service>
            {
                Success = false,
                ErrorMessage = "Token expired, please login again",
                StatusCode = 401
            };
        }

        return new ServiceResult<Service> 
        { 
            Success = true, 
            Data = service 
        };
    }

    // Subscribe a user
    public async Task<ServiceResult<SubscriptionResponse>> SubscribeAsync(SubscriptionRequest request)
    {
        var validationResult = await ValidateServiceAndTokenAsync(request.service_id, request.token_id);
        if (!validationResult.Success)
        {
            return new ServiceResult<SubscriptionResponse>
            {
                Success = false,
                ErrorMessage = validationResult.ErrorMessage,
                StatusCode = validationResult.StatusCode
            };
        }

        var service = validationResult.Data!;

        var existingSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.ServiceId == service.Id && s.PhoneNumber == request.phone_number);

        if (existingSubscription != null && !existingSubscription.IsActive)
        {
            existingSubscription.IsActive = true;
            existingSubscription.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new ServiceResult<SubscriptionResponse>
            {
                Success = true,
                Data = new SubscriptionResponse { subscription_id = existingSubscription.SubscriptionId },
                StatusCode = 200
            };
        }
        else if (existingSubscription != null && existingSubscription.IsActive)
        {
            return new ServiceResult<SubscriptionResponse>
            {
                Success = false,
                ErrorMessage = "Already subscribed",
                StatusCode = 409
            };
        }

        // Generate new subscription ID using Guid which is a .NET built-in method to generate unique identifiers
        var subscriptionId = Guid.NewGuid().ToString();
        var subscription = new Subscription
        {
            SubscriptionId = subscriptionId,
            ServiceId = service.Id,
            PhoneNumber = request.phone_number,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
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

    // Unsubscribe a user
    public async Task<ServiceResult<object>> UnsubscribeAsync(SubscriptionRequest request)
    {
        var validationResult = await ValidateServiceAndTokenAsync(request.service_id, request.token_id);
        if (!validationResult.Success)
        {
            return new ServiceResult<object>
            {
                Success = false,
                ErrorMessage = validationResult.ErrorMessage,
                StatusCode = validationResult.StatusCode
            };
        }

        var service = validationResult.Data!;

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
        subscription.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new ServiceResult<object>
        {
            Success = true,
            Data = new { message = "Unsubscribed successfully" },
            StatusCode = 200
        };
    }

    // Get subscription status
    public async Task<ServiceResult<StatusResponse>> GetStatusAsync(StatusRequest request)
    {
        var validationResult = await ValidateServiceAndTokenAsync(request.service_id, request.token_id);
        if (!validationResult.Success)
        {
            return new ServiceResult<StatusResponse>
            {
                Success = false,
                ErrorMessage = validationResult.ErrorMessage,
                StatusCode = validationResult.StatusCode
            };
        }

        var service = validationResult.Data!;

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
                subscription_date = subscription.UpdatedAt
            },
            StatusCode = 200
        };
    }

    // Delete a subscription
    public async Task<ServiceResult<object>> DeleteSubscriptionAsync(SubscriptionRequest request)
    {
        var validationResult = await ValidateServiceAndTokenAsync(request.service_id, request.token_id);
        if (!validationResult.Success)
        {
            return new ServiceResult<object>
            {
                Success = false,
                ErrorMessage = validationResult.ErrorMessage,
                StatusCode = validationResult.StatusCode
            };
        }

        var service = validationResult.Data!;

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.ServiceId == service.Id && s.PhoneNumber == request.phone_number);

        if (subscription == null)
        {
            return new ServiceResult<object>
            {
                Success = false,
                ErrorMessage = "Subscription not found",
                StatusCode = 404
            };
        }

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();

        return new ServiceResult<object>
        {
            Success = true,
            Data = new { message = "Subscription deleted successfully" },
            StatusCode = 200
        };
    }

    // Get all subscriptions
    public async Task<List<ServiceResult<Subscription?>>> GetAllSubscriptionsAsync()
    {
        var subscriptions = await _context.Subscriptions.ToListAsync();

        var results = new List<ServiceResult<Subscription?>>();
        if (subscriptions.Count == 0)
        {
            results.Add(new ServiceResult<Subscription?>
            {
                Success = false,
                Data = null,
                ErrorMessage = "No subscriptions found",
                StatusCode = 404
            });
            return results;
        }

        foreach (var subscription in subscriptions)
        {
            var result = new ServiceResult<Subscription?>
            {
                Success = true,
                Data = subscription
            };
            results.Add(result);
        }
        return results;
    }

    // Get filtered subscriptions by date range
    public async Task<List<ServiceResult<Subscription?>>> GetFilteredSubscriptionAsync(DateTime From, DateTime To)
    {
        var subscriptionsresult = await GetAllSubscriptionsAsync();
        var results = new List<ServiceResult<Subscription?>>();

        if (subscriptionsresult.All(s => s.Data == null))
        {
            results.Add(new ServiceResult<Subscription?>
            {
                Success = false,
                Data = null,
                ErrorMessage = "No subscriptions found",
                StatusCode = 404
            });
            return results;
        }
        var filteredSubscriptions = subscriptionsresult.Where(s => s.Data != null && s.Data.CreatedAt >= From && s.Data.CreatedAt <= To).ToList();

        if (filteredSubscriptions.Count == 0)
        {
            results.Add(new ServiceResult<Subscription?>
            {
                Success = false,
                Data = null,
                ErrorMessage = "No subscriptions found in the given date range",
                StatusCode = 404
            });
            return results;
        }

        foreach (var subscription in filteredSubscriptions)
        {
            var result = new ServiceResult<Subscription?>
            {
                Success = subscription.Success,
                Data = subscription.Data
            };
            results.Add(result);
        }
        return results;
    }
}
