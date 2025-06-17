using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.API.Controllers;

[ApiController]
[Route("api/stripe/webhook")]
public class StripeWebhookController : ControllerBase
{
    private readonly TalentLinkDbContext _context;
    private readonly IConfiguration _config;

    public StripeWebhookController(TalentLinkDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var secret = _config["Stripe:WebhookSecret"]!;
        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], secret);
        }
        catch (StripeException e)
        {
            return BadRequest($"Webhook error: {e.Message}");
        }


        if (stripeEvent.Type == "payment_intent.succeeded")
        {
            var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

            // Optional: anhand einer Custom ID den Job identifizieren
            var metadata = paymentIntent.Metadata;
            if (metadata.TryGetValue("jobId", out var jobIdString) && Guid.TryParse(jobIdString, out var jobId))
            {
                var job = await _context.Jobs.FindAsync(jobId);
                if (job != null)
                {
                    job.IsBoosted = true;
                    await _context.SaveChangesAsync();
                }
            }
        }

        return Ok();
    }
}
