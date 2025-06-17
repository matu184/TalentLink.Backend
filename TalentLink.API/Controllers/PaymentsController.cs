using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.IO;
using System.Threading.Tasks;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly TalentLinkDbContext _context;
    private readonly IConfiguration _config;

    public PaymentController(TalentLinkDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var endpointSecret = _config["Stripe:WebhookSecret"];

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                endpointSecret
            );

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                var metadata = paymentIntent?.Metadata;

                if (metadata != null && metadata.TryGetValue("jobId", out var jobIdStr))
                {
                    if (Guid.TryParse(jobIdStr, out var jobId))
                    {
                        var job = await _context.Jobs.FindAsync(jobId);
                        if (job != null)
                        {
                            job.IsPaid = true;
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest($"⚠️ Webhook-Fehler: {ex.Message}");
        }
    }
}
