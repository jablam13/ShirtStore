using StoreModel.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Interface
{
    public interface IAddressService
    {
        Task<UserAddress> CreateAddress(UserAddress address);
    }
}
