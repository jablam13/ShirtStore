using StoreModel.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreService.Interface
{
    public interface IStoreService
    {
        Store GetStore(Guid storeUid, Guid userUid, Guid visitorUid);
        StoreItem GetStoreItem(Guid storeUid, Guid userUid, Guid visitorUid);
    }
}
