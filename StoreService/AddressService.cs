using StoreModel.Account;
using StoreRepository.Interface;
using StoreService.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreService
{
    public class AddressService : BaseService, IAddressService
    {
        private readonly IAddressRepository addressRep;

        public AddressService(IAddressRepository _addressRep)
        {
            addressRep = _addressRep;
        }

        public UserAddress AddAddress(UserAddress address)
        {
            return addressRep.CreateAddress(address);
        }
        public UserAddress EditAddress(UserAddress address)
        {
            return addressRep.EditAddress(address);
        }
        public bool RemoveAddress(UserAddress address)
        {
            bool success = false;

            try
            {
                addressRep.EditAddress(address);
                success = true;
            }
            catch (Exception ex)
            {
                //Log exception
                success = false;
            }
            return success;
        }

        public UserAddress GetAddress(UserAddress address)
        {
            return addressRep.GetAddress(address);
        }

        public UserAddress GetAddress(int addressId)
        {
            return addressRep.GetAddress(addressId);
        }
        public UserAddress GetAddress(Guid addressUid)
        {
            return addressRep.GetAddress(addressUid);
        }
    }
}
