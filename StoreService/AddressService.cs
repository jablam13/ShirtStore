using StoreModel.Account;
using StoreRepository.Interface;
using StoreService.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoreService
{
    public class AddressService : BaseService, IAddressService
    {
        private readonly IAddressRepository addressRep;

        public AddressService(IAddressRepository _addressRep)
        {
            addressRep = _addressRep;
        }

        public async Task<UserAddress> CreateAddress(UserAddress address)
        {
            if (!string.IsNullOrWhiteSpace(address.Street) && !string.IsNullOrWhiteSpace(address.City))
            {
                address = await addressRep.CreateAddress(address).ConfigureAwait(false);
            }

            return address;
        }

    }
}
