using StoreModel.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreRepository.Interface
{
    public interface IAddressRepository
    {
        Task<UserAddress> CreateAddress(UserAddress address);
        Task<UserAddress> EditAddress(UserAddress address);
        Task<Guid?> RemoveAddress(Guid addressUid, Guid userUid);
        Task<List<UserAddress>> GetUserAddresses(UserAddress address);
        Task<UserAddress> GetAddressById(int addressId);
    }
}
