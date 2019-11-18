using StoreModel.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreRepository.Interface
{
    public interface IStoreRepository
    {
        List<Store> GetStores(int userId);
        Store GetStoreAll(Guid? userUid, Guid? visitorUid, Guid storeGuid);
        Store GetStore(Guid storeUid);
        List<StoreItem> GetStoreItems(Guid? userUid, Guid? visitorUid, Guid storeUid, int? categoryId);
        StoreItem GetStoreItem(Guid? userUid, Guid? visitorUid, Guid storeItemUid);
    }
}
