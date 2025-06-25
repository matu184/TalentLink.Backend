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
<<<<<<< HEAD

=======
>>>>>>> heroku/main
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

<<<<<<< HEAD
            // Preislogik: 1€ für Erstellen, 2€ für Boost
            long amountInCents = request.IsBoost ? 300 : 100;
            string productName = request.IsBoost ? "Job Boost" : "Job Erstellung";
            string productDescription = request.IsBoost ? $"Boost für Job: {job.Title}" : $"Erstellung für Job: {job.Title}";

=======
>>>>>>> heroku/main
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
<<<<<<< HEAD
                            UnitAmount = amountInCents, // 100 = 1€, 200 = 2€
                            Currency = "eur",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = productName,
                                Description = productDescription
=======
                            UnitAmount = request.Amount, // Betrag in Cent (z.B. 499 für 4,99€)
                            Currency = "eur",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Job Boost",
                                Description = $"Boost für Job: {job.Title}"
>>>>>>> heroku/main
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
<<<<<<< HEAD
                SuccessUrl = request.SuccessUrl ?? "https://c09e-84-171-168-238.ngrok-free.app/successpage",                    
                CancelUrl = request.CancelUrl ?? "https://c09e-84-171-168-238.ngrok-free.app ",
                Metadata = new Dictionary<string, string>
                {
                    ["jobId"] = request.JobId.ToString(),
                    ["isBoost"] = request.IsBoost.ToString()
=======
                SuccessUrl = $"{_config["Frontend:BaseUrl"]}/payment/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_config["Frontend:BaseUrl"]}/payment/cancel",
                Metadata = new Dictionary<string, string>
                {
                    ["jobId"] = request.JobId.ToString()
>>>>>>> heroku/main
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
<<<<<<< HEAD

=======
>>>>>>> heroku/main
    // DTOs für die API
    public class CreateSessionRequest
    {
        public Guid JobId { get; set; }
<<<<<<< HEAD
        public bool IsBoost { get; set; } // true = Boost, false = Erstellung
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }
=======
        public long Amount { get; set; } // Betrag in Cent
>>>>>>> heroku/main
    }

    public class CreateSessionResponse
    {
        public string Url { get; set; } = string.Empty;
    }
}
