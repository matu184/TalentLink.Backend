using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
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
    [HttpPost("create-session")]
    [Authorize] // Falls du Authorization brauchst
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request)
    {
        try
        {
            // Prüfe ob der Job existiert
            var job = await _context.Jobs.FindAsync(request.JobId);
            if (job == null)
            {
                return NotFound("Job nicht gefunden");
            }

            // Erstelle Stripe Checkout Session
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = request.Amount, // Betrag in Cent (z.B. 499 für 4,99€)
                            Currency = "eur",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Job Boost",
                                Description = $"Boost für Job: {job.Title}"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = $"{_config["Frontend:BaseUrl"]}/payment/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_config["Frontend:BaseUrl"]}/payment/cancel",
                Metadata = new Dictionary<string, string>
                {
                    ["jobId"] = request.JobId.ToString()
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Ok(new CreateSessionResponse
            {
                Url = session.Url
            });
        }
        catch (StripeException ex)
        {
            return BadRequest($"Stripe Fehler: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server Fehler: {ex.Message}");
        }
    }
    // DTOs für die API
    public class CreateSessionRequest
    {
        public Guid JobId { get; set; }
        public long Amount { get; set; } // Betrag in Cent
    }

    public class CreateSessionResponse
    {
        public string Url { get; set; } = string.Empty;
    }
}
