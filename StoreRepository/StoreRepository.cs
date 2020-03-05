using System;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using StoreRepository.Interface;
using StoreModel.Generic;
using StoreModel.Store;

namespace StoreRepository
{
    public class StoreRepository : BaseRepository, IStoreRepository
    {
        private readonly int siteId;

        public StoreRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
            siteId = appSettings.Value.SiteId;
        }

        public List<Store> GetStores(int userId)
        {
			const string sql = @"
SELECT s.Id, s.[Uid],
s.Name, s.[Description], s.LargeImg, s.SmallImg, s.Active, s.CreatedDate, s.LastModifiedDate
FROM Store s 
JOIN StoreCollection sc ON sc.StoreId = s.Id 
LEFT JOIN SiteUser u on u.Id = s.CreatorId 
WHERE 1=1
AND s.Active = 1;
";

            var stores = Query<Store>(sql, new { Id = userId }).ToList();

            return stores;
        }

        public Store GetStoreAll(Guid? userUid, Guid? visitorUid, Guid storeGuid)
        {
            const string sql = @"
SELECT s.Id, s.[Uid],
s.Name, s.[Description], s.LargeImg, s.SmallImg, s.Active, s.CreatedDate, s.LastModifiedDate
FROM Store s 
JOIN StoreCollection sc ON sc.StoreId = s.Id 
LEFT JOIN SiteUser u on u.Id = s.CreatorId 
WHERE 1=1
AND sc.SiteCollectionId = @SiteId
AND s.Active = 1;
";
            var store = Query<Store>(sql, new { SiteId = siteId }).FirstOrDefault();

            store.StoreItems = GetStoreItems(userUid, visitorUid, store.Uid, null);

            return store;
        }

        public Store GetStore(Guid storeUid)
        {
            const string sql = @"
SELECT s.Id, s.[Uid], u.Id as CreatorId, u.FirstName + ' ' + SUBSTRING(u.LastName,0,2) + '.' as CreatorName, 
s.Name, s.[Description], s.LargeImg, s.SmallImg, s.Active, s.CreatedDate, s.LastModifiedDate
FROM Store s 
JOIN StoreCollection sc ON sc.StoreId = s.Id 
LEFT JOIN SiteUser u on u.Id = s.CreatorId 
WHERE 1=1
AND s.[Uid] = @StoreGuid
AND sc.SiteCollectionId = @SiteId
AND s.Active = 1;
";

            return Query<Store>(sql, new { StoreUid = storeUid, SiteId = siteId }).FirstOrDefault();
        }

        public List<StoreItem> GetStoreItems(Guid? userUid, Guid? visitorUid, Guid storeUid, int? categoryId)
        {
            const string sql = @"
;WITH UserCartModel (Id, Uid, Price, Quantity, StoreItemId) AS (
	SELECT c.Id, c.[Uid],
	si.Price, ci.Quantity as Quantity, si.Id AS StoreItemId
	FROM Cart c LEFT JOIN CartItem ci on ci.CartId = c.Id
	LEFT JOIN StoreItem si on si.Id = ci.ItemId
	WHERE c.UserUid = @UserUid OR (@UserUid IS NULL AND c.VisitorUid = @VisitorUid)
	GROUP BY c.Id, c.[Uid], si.Id, si.Price, ci.Quantity, si.Id
),
VisitorCartModel (Id, Uid, Price, Quantity, StoreItemId) AS (
	SELECT c.Id, c.[Uid],
	si.Price, ci.Quantity as Quantity, si.Id AS StoreItemId
	FROM Cart c LEFT JOIN CartItem ci on ci.CartId = c.Id
	LEFT JOIN StoreItem si on si.Id = ci.ItemId
	WHERE c.VisitorUid = @VisitorUid
	GROUP BY c.Id, c.[Uid], si.Id, si.Price, ci.Quantity, si.Id
),
TotalCart AS (
SELECT 
	
	CASE WHEN (@UserUid IS NULL)  THEN vc.Id ELSE uc.Id END AS Id,
	CASE WHEN (@UserUid IS NULL) THEN vc.[Uid] ELSE uc.[Uid] END AS Uid,
	CASE WHEN ISNULL(v.Price, 0) < ISNULL(u.Price, 0) THEN u.Price ELSE v.Price END AS Price,
	CASE WHEN ISNULL(u.Quantity, 0) > ISNULL(v.Quantity, 0) THEN ISNULL(u.Quantity, 0) ELSE ISNULL(v.Quantity, 0) END AS Quantity,
	ci.ItemId AS StoreItemId,
	ci.Id AS CartItemId
	FROM CartItem ci LEFT JOIN UserCartModel u on u.StoreItemId = ci.ItemId 
	LEFT JOIN VisitorCartModel v on v.StoreItemId = ci.ItemId
	LEFT JOIN Cart c on c.Id = ci.CartId
	LEFT JOIN Cart uc on uc.UserUid = @UserUid 
	LEFT JOIN Cart vc on vc.VisitorUid = @VisitorUid 
	WHERE c.VisitorUid = @VisitorUid OR c.UserUid = @UserUid 
)

SELECT DISTINCT si.StoreId, si.Id as Id, si.[Uid] as [Uid], s.[Uid] as StoreUid, si.Name, 
si.[Description], si.Price, ISNULL(si.Quantity, 0) - ISNULL(tc.Quantity, 0) as Quantity,
si.Active as [ItemActive], si.Active as [StoreItemActive], si.Quantity as TotalQuantity
FROM StoreItem si 
LEFT JOIN TotalCart tc on tc.StoreItemId = si.Id
LEFT JOIN Store s on s.Id = si.StoreId 
WHERE s.[Uid] = @StoreUid;
";
            var user = Query<StoreItem>(sql, new { UserUid = userUid, VisitorUid = visitorUid, StoreUid = storeUid, CategoryId = categoryId, SiteId = siteId }).ToList();

            return user;
        }

        public StoreItem GetStoreItem(Guid? userUid, Guid? visitorUid, Guid storeItemUid)
        {
            const string sql = @"
;WITH UserCartModel (Id, Uid, Price, Quantity, StoreItemId) AS (
	SELECT c.Id, c.[Uid],
	si.Price, ci.Quantity as Quantity, si.Id AS StoreItemId
	FROM Cart c LEFT JOIN CartItem ci on ci.CartId = c.Id
	LEFT JOIN StoreItem si on si.Id = ci.ItemId
	WHERE c.UserUid = @UserUid OR (@UserUid IS NULL AND c.VisitorUid = @VisitorUid)
	GROUP BY c.Id, c.[Uid], si.Id, si.Price, ci.Quantity, si.Id
),
VisitorCartModel (Id, Uid, Price, Quantity, StoreItemId) AS (
	SELECT c.Id, c.[Uid],
	si.Price, ci.Quantity as Quantity, si.Id AS StoreItemId
	FROM Cart c LEFT JOIN CartItem ci on ci.CartId = c.Id
	LEFT JOIN StoreItem si on si.Id = ci.ItemId
	WHERE c.VisitorUid = @VisitorUid
	GROUP BY c.Id, c.[Uid], si.Id, si.Price, ci.Quantity, si.Id
),
TotalCart AS (
SELECT 
	
	CASE WHEN (@UserUid IS NULL)  THEN vc.Id ELSE uc.Id END AS Id,
	CASE WHEN (@UserUid IS NULL) THEN vc.[Uid] ELSE uc.[Uid] END AS Uid,
	CASE WHEN ISNULL(v.Price, 0) < ISNULL(u.Price, 0) THEN u.Price ELSE v.Price END AS Price,
	CASE WHEN ISNULL(u.Quantity, 0) > ISNULL(v.Quantity, 0) THEN ISNULL(u.Quantity, 0) ELSE ISNULL(v.Quantity, 0) END AS Quantity,
	ci.ItemId AS StoreItemId,
	ci.Id AS CartItemId
	FROM CartItem ci LEFT JOIN UserCartModel u on u.StoreItemId = ci.ItemId 
	LEFT JOIN VisitorCartModel v on v.StoreItemId = ci.ItemId
	LEFT JOIN Cart c on c.Id = ci.CartId
	LEFT JOIN Cart uc on uc.UserUid = @UserUid 
	LEFT JOIN Cart vc on vc.VisitorUid = @VisitorUid 
	WHERE c.VisitorUid = @VisitorUid OR c.UserUid = @UserUid 
)

SELECT DISTINCT si.StoreId, si.Id as Id, si.[Uid] as [Uid], s.[Uid] as StoreUid, si.Name, 
si.[Description], si.Price, ISNULL(si.Quantity, 0) - ISNULL(tc.Quantity, 0) as Quantity,
si.Active as [ItemActive], si.Active as [StoreItemActive], si.Quantity as TotalQuantity
FROM StoreItem si 
LEFT JOIN TotalCart tc on tc.StoreItemId = si.Id
LEFT JOIN Store s on s.Id = si.StoreId 
WHERE si.[Uid] = @StoreItemUid;";
            var storeItem = Query<StoreItem>(sql, new { UserUid = userUid, VisitorUid = visitorUid, StoreItemUid = storeItemUid }).FirstOrDefault();

            return storeItem;
        }
    }
}
