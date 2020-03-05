using Microsoft.Extensions.Options;
using StoreModel.Generic;
using StoreModel.Store;
using StoreRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreRepository
{
    public class CartRepository : BaseRepository, ICartRepository
    {
        public CartRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        { }

        public Cart GetCartAll(Guid visitorUid, Guid userUid)
        {
            var cart = GetCart(visitorUid, userUid);
            cart.CartItems = GetCartItems(cart.Uid);

            return cart;
        }

        public Cart GetCart(Guid visitorUid, Guid userUid)
        {
            string sql = @"
IF @VisitorUid IS NOT NULL AND NOT EXISTS(SELECT * FROM Cart WHERE VisitorUid = @VisitorUid)
BEGIN
	INSERT INTO Cart
	SELECT NEWID(), GETDATE(),GETDATE(),@VisitorUid, @UserUid, Id FROM SiteUser WHERE [Uid] = @UserUid;
END;

IF @UserUid IS NOT NULL AND NOT EXISTS(SELECT * FROM Cart WHERE UserId = @UserUid)
BEGIN
	IF EXISTS(SELECT * FROM Cart WHERE VisitorUid = @VisitorUid)
	BEGIN 
		UPDATE Cart SET UserUid = @UserUid, UserId = (SELECT Id FROM SiteUser WHERE [Uid] = @UserUid AND SiteId = @SiteId) WHERE UserUid = @UserUid;
	END
	ELSE 
	BEGIN
		INSERT INTO Cart
		SELECT NEWID(), GETDATE(),GETDATE(),@VisitorUid, @UserUid, Id FROM SiteUser WHERE [Uid] = @UserUid;
	END
END;

IF @UserUid IS NULL
BEGIN
	SELECT Uid, CreatedDate, LastModifiedDate, VisitorUid, UserUid, UserId FROM Cart WHERE UserUid = @UserUid;
END 
ELSE
	SELECT Uid, CreatedDate, LastModifiedDate, VisitorUid, UserUid, UserId FROM Cart WHERE VisitorUid = @VisitorUid;
";

            return Query<Cart>(sql, new {
                VisitorUid = visitorUid,
                UserUid = userUid,
                SiteId = siteId,
            }).FirstOrDefault();
        }

        public Cart GetVisitorCart(Guid visitorUid)
        {
            string sql = "SELECT Uid, CreatedDate, LastModifiedDate, VisitorUid, UserUid, UserId FROM Cart WHERE VisitorUid = @VisitorUid;";

            return Query<Cart>(sql, new
            {
                VisitorUid = visitorUid,
                SiteId = siteId,
            }).FirstOrDefault();
        }

        public Cart GetUserCart(Guid userUid)
        {
            string sql = "SELECT Uid, CreatedDate, LastModifiedDate, VisitorUid, UserUid, UserId FROM Cart WHERE UserUid = @UserUid; ";

            return Query<Cart>(sql, new
            {
                UserUid = userUid,
                SiteId = siteId,
            }).FirstOrDefault();
        }

        public List<CartItem> GetCartItems(int cartId)
        {
            const string sql = @"
SELECT Id, [Uid], CartId, ItemId, Quantity, CreatedDate, LastModifiedDate, Active 
FROM CartItem WHERE [CartId] = (SELECT Id FROM Cart WHERE Uid = @CartUid);";

            return Query<CartItem>(sql, new { CartId = cartId }).ToList();
        }

        public List<CartItem> GetCartItems(Guid cartUid)
        {
            const string sql = @"
SELECT Id, [Uid], CartId, ItemId, Quantity, CreatedDate, LastModifiedDate, Active 
FROM CartItem WHERE [CartId] = (SELECT Id FROM Cart WHERE Uid = @CartUid);";

            return Query<CartItem>(sql, new { CartUid = cartUid }).ToList();
        }

        public CartItem AddCartItem(StoreItem item, int cartId)
        {
            const string sql = @"
DECLARE @CreatedCartItem TABLE ([Id] UNIQUEIDENTIFIER);
		
IF EXISTS(SELECT * FROM CartItem WHERE CartId = @CartId AND ItemId = @ItemId)
BEGIN 
	UPDATE CartItem 
	SET Quantity = @Quantity 
	OUTPUT INSERTED.Id INTO @CreatedCartItem
	WHERE CartId = @CartId AND ItemId = @ItemId
END
ELSE 
BEGIN
	INSERT INTO CartItem 
	OUTPUT INSERTED.Id INTO @CreatedCartItem
	SELECT NEWID(), @CartId, @ItemId, @Quantity, GETDATE(), GETDATE(), 1 
END;

SELECT * FROM CartItem WHERE Id = (SELECT TOP 1 Id FROM @CreatedCartItem);
";

            var cartItem = Query<CartItem>(sql,
                new
                {
                    CartId = cartId,
                    ItemId = item.Id,
                    item.Quantity
                })
                .FirstOrDefault();
            return cartItem;
        }

        public Cart UpdateCart(Cart cart)
        {
            return new Cart();
        }

        public CartItem EditCartItem(Cart cart)
        {
            const string sql = @"

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
            return new CartItem();
        }

        public bool RemoveCart(Cart cart)
        {
            bool success = false;
            const string sql = "";

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

        public bool RemoveCartItem(CartItem cartItem)
        {
            bool success = false;
            const string sql = "";

            try
            {
                success = Convert.ToBoolean(Execute(sql, cartItem));
            }
            catch (Exception ex)
            {
                //Log Exception
            }
            return success;
        }

        public bool CartExists(Guid userUid, Guid visitorUid)
        {
            const string sql = "SELECT EXIST(SELECT * FROM Cart WHERE UserUid = @UserUid OR VisitorUid = @VisitorUid);";
            return Query<bool>(sql,
                new
                {
                    UserUid = userUid,
                    VisitorUid = visitorUid
                })
                .FirstOrDefault();
        }

        public Cart MergeCarts(Guid userUid, Guid visitorUid)
        {
            const string sql = "";
            return Query<Cart>(sql,
                new
                {
                    UserUid = userUid,
                    VisitorUid = visitorUid
                })
                .FirstOrDefault();
        }
    }
}
