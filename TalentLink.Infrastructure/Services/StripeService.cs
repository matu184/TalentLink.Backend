using Microsoft.Extensions.Configuration;
using Stripe.Checkout;

public class StripeService
{
    private readonly IConfiguration _config;

    public StripeService(IConfiguration config)
    {
        _config = config;
    }

    public Session CreatePaymentSession(Guid jobId, decimal amount)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "eur",
                        UnitAmountDecimal = amount * 100, // in Cent
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Job-Erstellung auf TalentLink"
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = "https://localhost:44319/payment/success?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = "https://localhost:44319/payment/cancel",
            Metadata = new Dictionary<string, string>
            {
                { "jobId", jobId.ToString() },
                { "purpose", "create" } // alternativ: "boost"
            }
        };

        var service = new SessionService();
        return service.Create(options);
    }
}
