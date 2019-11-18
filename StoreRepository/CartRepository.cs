using Microsoft.Extensions.Options;
using StoreModel.Generic;
using StoreModel.Store;
using StoreRepository.Interface;
using System;
using System.Linq;

namespace StoreRepository
{
    public class CartRepository : BaseRepository, ICartRepository
    {
        private readonly static int siteId = 12;

        public CartRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public Cart GetCart(Guid userUid)
        {
            return new Cart();
        }
        public Cart GetCart(Guid? userUid, Guid? cartUid)
        {

            return new Cart();
        }
        public Cart AddCart(Cart cart)
        {
            return new Cart();
        }
        public Cart AddCartItem(StoreItem item)
        {
            var sql = @"
DECLARE @StoreQuantity INT = (SELECT Quantity FROM StoreItemInfo WHERE [Uid] = @StoreItemUid),
		@ItemId INT = (SELECT Id FROM StoreItemInfo WHERE [Uid] = @StoreItemUid),
		@CartId INT = NULL;

IF @UserUid IS NOT NULL 
BEGIN 
	IF NOT EXISTS(SELECT * FROM Cart WHERE UserUid = @UserUid)
	BEGIN
		INSERT INTO CART
		SELECT NEWID(), GETDATE(), GETDATE(), @VisitorUid, @UserUid
	END

	SET @CartId = (SELECT Id FROM Cart WHERE UserUid = @UserUid) 
END

IF @VisitorUid IS NOT NULL 
BEGIN 
	IF NOT EXISTS(SELECT * FROM Cart WHERE VisitorUid = @VisitorUid)
	BEGIN
		INSERT INTO CART
		SELECT NEWID(), GETDATE(), GETDATE(), @VisitorUid, @UserUid
	END
	
	SET @CartId = (SELECT Id FROM Cart WHERE UserUid = @VisitorUid) 
END

IF(@CartId IS NOT NULL)
    BEGIN
    IF EXISTS(SELECT * FROM CartItem WHERE CartId = @CartId AND ItemId = @ItemId)
    BEGIN 
	    UPDATE CartItem SET Quantity = (CASE
		    WHEN @Quantity + Quantity > @StoreQuantity THEN @StoreQuantity
		    ELSE @Quantity + Quantity
	    END)  WHERE CartId = @CartId AND ItemId = @ItemId; 
    END 
    ELSE 
    BEGIN
	    INSERT INTO CartItem
	    SELECT TOP 1 NEWID(), @CartId, @ItemId, @Quantity, GETDATE(), GETDATE(), 1;
    END
END;
";

            var cartItem = Query<StoreItem>(sql, new { StoreItemUid = item.Uid, Quantity = item.Quantity, Price = item.Price }).ToList();
            return new Cart();
        }
        public Cart EditCart(Cart cart)
        {
            return new Cart();
        }
        public Cart EditCartItem(Cart cart)
        {
            var sql = @"

DECLARE @UserUid UNIQUEIDENTIFIER = (SELECT [Uid] FROM SiteUser WHERE EmailAddress = 'testuser11@addabadda.com'),
		@VisitorUid UNIQUEIDENTIFIER = '52283B5C-A03F-4C53-8C58-986776F80014',
		@StoreItemUid UNIQUEIDENTIFIER = '64BF53A8-6FAC-4687-AEB5-10710DFFB1D0',
		@Quantity INT = 2,
		@Price DECIMAL = 14.99,
		@CartId INT = NULL; 

DECLARE @StoreQuantity INT = (SELECT Quantity FROM StoreItemInfo WHERE [Uid] = @StoreItemUid),
		@ItemId INT = (SELECT Id FROM StoreItemInfo WHERE [Uid] = @StoreItemUid);

IF @UserUid IS NOT NULL 
BEGIN 
	IF NOT EXISTS(SELECT * FROM Cart WHERE UserUid = @UserUid)
	BEGIN
		INSERT INTO CART
		SELECT NEWID(), GETDATE(), GETDATE(), @VisitorUid, @UserUid
	END

	SET @CartId = (SELECT Id FROM Cart WHERE UserUid = @UserUid) 
END

IF @VisitorUid IS NOT NULL 
BEGIN 
	IF NOT EXISTS(SELECT * FROM Cart WHERE VisitorUid = @VisitorUid)
	BEGIN
		INSERT INTO CART
		SELECT NEWID(), GETDATE(), GETDATE(), @VisitorUid, @UserUid
	END
	
	SET @CartId = (SELECT Id FROM Cart WHERE UserUid = @VisitorUid) 
END

IF EXISTS(SELECT * FROM CartItem WHERE CartId = @CartId AND ItemId = @ItemId)
BEGIN 
	UPDATE CartItem SET Quantity = CASE
		WHEN @Quantity < 0 THEN 0 
		WHEN @Quantity > @StoreQuantity THEN @StoreQuantity
		ELSE @Quantity 
		END, LastModifiedDate = GETDATE()
	WHERE CartId = @CartId
	AND ItemId = @ItemId;  
END 


SELECT sii.Id, sii.[Uid], sii.StoreId, si.Id as StoreItemId, si.[Uid] StoreItemUid, 
	ISNULL(sii.CategoryId, -1) as CategoryId, si.Name, si.[Description], sii.Price, 
	sii.Quantity - ISNULL(ci.Quantity, 0) as Quantity, 
	si.Active as [ItemActive], sii.Active as [StoreItemActive], sii.Quantity as TotalQuantity
	FROM StoreItem si 
	LEFT JOIN StoreItemInfo sii on sii.StoreItemId = si.Id 
	LEFT JOIN CartItem ci on ci.ItemId = sii.Id
	LEFT JOIN Cart c on c.Id = ci.CartId 
	WHERE sii.[Uid] = @StoreItemUid
    AND ci.Active = 1
	AND c.Id = @CartId;
";
            return new Cart();
        }
        public bool RemoveCart(Cart cart)
        {
            bool success = false;
            var sql = @"";

            try
            {
                success = Convert.ToBoolean(Execute(sql, cart));
            }
            catch (Exception ex)
            {
                //Log Exception
            }
            return success;
        }
        public bool RemoveCartItem(Cart cart)
        {
            bool success = false;
            var sql = @"";

            try
            {
                success = Convert.ToBoolean(Execute(sql, cart));
            }
            catch (Exception ex)
            {
                //Log Exception
            }
            return success;
        }
    }
}
