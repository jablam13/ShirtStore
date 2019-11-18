using StoreModel.Store;
using StoreRepository.Interface;
using StoreService.Interface;
using System;

namespace StoreService
{
    public class StoreService : IStoreService
    {

        private readonly IStoreRepository userRep;

        public StoreService(IStoreRepository _userRep)
        {
            userRep = _userRep;
        }

        public Store GetStore(Guid storeUid, Guid userUid, Guid visitorUid)
        {
            return userRep.GetStoreAll(userUid, visitorUid, storeUid);
        }

        public StoreItem GetStoreItem(Guid storeUid, Guid userUid, Guid visitorUid)
        {
            return userRep.GetStoreItem(userUid, visitorUid, storeUid);
        }
    }
}
