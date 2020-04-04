using Microsoft.Extensions.Options;
using StoreModel.Generic;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentService.Stripe
{
    public class StripeService : IStripeService
    {
        public StripeService()
        {
        }

        public void TestMethod()
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = 1000,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                ReceiptEmail = "jenny.rosen@example.com",
            };
            var service = new PaymentIntentService();
            service.Create(options);
        }

        public async Task<string> CreateSessionTest()
        {
            // Set your secret key. Remember to switch to your live secret key in production!
            // See your keys here: https://dashboard.stripe.com/account/apikeys

            var options = new PaymentIntentCreateOptions
            {
                Amount = 1099,
                Currency = "usd",
                // Verify your integration in this guide by including this parameter
                Metadata = new Dictionary<string, string>()
    {
      {"integration_check", "accept_a_payment"},
    }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options).ConfigureAwait(false);
            return paymentIntent.ClientSecret;
        }

        public async Task<string> CreateSession(List<StoreModel.Checkout.OrderItem> orderItems)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {
        "card",
    },
                LineItems = new List<SessionLineItemOptions> {
        new SessionLineItemOptions {
            Name = "T-shirt",
            Description = "Comfortable cotton t-shirt",
            Amount = 500,
            Currency = "usd",
            Quantity = 1,
        },
    },
                SuccessUrl = "https://example.com/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://example.com/cancel",
            };

            var service = new SessionService();

            Session session = await service.CreateAsync(options).ConfigureAwait(false);

            return session.Id;
        }
    }
}
