using Microsoft.AspNetCore.Mvc;
using SubscriptionService.DTOs;
using SubscriptionService.Services;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost("subscribe")]
    public async Task<ActionResult<SubscriptionResponse>> Subscribe([FromBody] SubscriptionRequest request)
    {
        if (string.IsNullOrEmpty(request.service_id) || string.IsNullOrEmpty(request.token_id) || string.IsNullOrEmpty(request.phone_number))
        {
            return BadRequest(new { message = "service_id, token_id, and phone_number are required" });
        }

        var result = await _subscriptionService.SubscribeAsync(request);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    [HttpPost("unsubscribe")]
    public async Task<ActionResult> Unsubscribe([FromBody] SubscriptionRequest request)
    {
        if (string.IsNullOrEmpty(request.service_id) || string.IsNullOrEmpty(request.token_id) || string.IsNullOrEmpty(request.phone_number))
        {
            return BadRequest(new { message = "service_id, token_id, and phone_number are required" });
        }

        var result = await _subscriptionService.UnsubscribeAsync(request);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    [HttpPost("status")]
    public async Task<ActionResult<StatusResponse>> GetStatus([FromBody] StatusRequest request)
    {
        if (string.IsNullOrEmpty(request.service_id) || string.IsNullOrEmpty(request.token_id) || string.IsNullOrEmpty(request.phone_number))
        {
            return BadRequest(new { message = "service_id, token_id, and phone_number are required" });
        }

        var result = await _subscriptionService.GetStatusAsync(request);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }
}
