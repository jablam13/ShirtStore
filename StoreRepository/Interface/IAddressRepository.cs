using StoreModel.Account;
using System;

namespace StoreRepository.Interface
{
    public interface IAddressRepository
    {
        UserAddress CreateAddress(UserAddress address);
        UserAddress EditAddress(UserAddress address);
        void RemoveAddress(UserAddress address);
        UserAddress GetAddress(UserAddress address);
        UserAddress GetAddress(int addressId);
        UserAddress GetAddress(Guid addressUid);
    }
}
