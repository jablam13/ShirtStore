using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
