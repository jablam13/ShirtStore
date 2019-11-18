using StoreModel.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreService.Interface
{
    public interface IAddressService
    {
        UserAddress AddAddress(UserAddress address);
        UserAddress EditAddress(UserAddress address);
        bool RemoveAddress(UserAddress address);
        UserAddress GetAddress(UserAddress address);
        UserAddress GetAddress(int addressId);
        UserAddress GetAddress(Guid addressUid);
    }
}
