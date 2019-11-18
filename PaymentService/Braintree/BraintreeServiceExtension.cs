using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace PaymentService.Braintree
{
    public static class BraintreeServiceExtension
    {
        public static IServiceCollection AddBraintree(this IServiceCollection collection, IConfiguration config)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (config == null) throw new ArgumentNullException(nameof(config));

            //collection.Configure<AccountServiceOptions>(config);
            collection.AddTransient<IBraintreeService, BraintreeService>();
            return collection;
        }
    }
}
