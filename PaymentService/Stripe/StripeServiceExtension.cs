using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System;

namespace PaymentService.Stripe
{
    public static class StripeServiceExtension
    {
        public static IServiceCollection AddStripe(this IServiceCollection collection, IConfiguration config)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (config == null) throw new ArgumentNullException(nameof(config));

            StripeConfiguration.ApiKey = config.GetSection("Stripe")["SecretKey"];
            collection.AddTransient<IStripeService, StripeService>();
            return collection;
        }
    }
}
