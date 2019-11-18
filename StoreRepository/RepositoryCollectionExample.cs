using StoreRepository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace StoreRepository
{
    public static class RepositoryCollectionExtension
    {
        public static IServiceCollection AddRepositoryCollection(this IServiceCollection collection, IConfiguration config)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (config == null) throw new ArgumentNullException(nameof(config));

            //collection.Configure<AccountServiceOptions>(config);
            collection.AddTransient<IUsersRepository, UsersRepository>();
            collection.AddTransient<IOrderRepository, OrderRepository>();
            collection.AddTransient<IAddressRepository, AddressRepository>();
            collection.AddTransient<ICartRepository, CartRepository>();
            collection.AddTransient<IOrderRepository, OrderRepository>();
            collection.AddTransient<IStoreRepository, StoreRepository>();
            return collection;
        }
    }
}
