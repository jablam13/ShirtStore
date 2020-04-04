
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreService.Interface;
using System;

namespace StoreService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceCollection(this IServiceCollection collection, IConfiguration config)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (config == null) throw new ArgumentNullException(nameof(config));

            // collection.Configure<AccountServiceOptions>(config);
            collection.AddScoped<IUserVisitorService, UserVisitorService>();
            collection.AddTransient<IAccountService, AccountService>();
            collection.AddTransient<ICartService, CartService>();
            collection.AddTransient<IOrderService, OrderService>();
            collection.AddTransient<IAddressService, AddressService>();
            collection.AddTransient<IStoreService, StoreService>();
            return collection;
        }
    }
}
