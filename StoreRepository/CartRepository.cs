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

        public Cart GetCartAll(Guid uid, bool isVisitor)
        {
            var cart = GetCart(uid, isVisitor);
            cart.CartItems = GetCartItems(cart.Uid);

            return cart;
        }

        public Cart GetCart(Guid uid, bool isVisitor)
        {
            var userType = isVisitor ? "VisitorUid" : "UserUid";
            string sql = $"SELECT * FROM Cart WHERE {userType} = @Uid;";

            return Query<Cart>(sql, new { Uid = uid }).FirstOrDefault();
        }

        public List<CartItem> GetCartItems(Guid cartUid)
        {
            const string sql = "SELECT * FROM CartItem WHERE [CartId] = (SELECT Id FROM Cart WHERE Uid = @CartUid);";

            return Query<CartItem>(sql, new { CartUid = cartUid }).ToList();
        }

        public List<CartItem> GetCartItems(Guid uid, bool isVisitor)
        {
            var userType = isVisitor ? "VisitorUid" : "UserUid";
            string sql = $@"
DECLARE @CartId INT;
SELECT @CartId = [Uid] FROM Cart WHERE ${userType} = @Uid;

SELECT * FROM CartItem WHERE CartId = @CartId;";
            return Query<CartItem>(sql, new { Uid = uid }).ToList();
        }
        
        public Cart CreateCart(Guid? userUid = null, Guid? visitorUid = null)
        {
            const string sql = @"
DECLARE	@UserId INT = (SELECT Id From SiteUser WHERE [Uid] = @UserUid);

IF NOT EXISTS(SELECT * FROM Cart WHERE VisitorUid = @VisitorUid OR UserUid = @UserUid)
BEGIN
    DECLARE @CreatedCart TABLE ([CartUid] UNIQUEIDENTIFIER);
    INSERT INTO Cart 
    OUTPUT INSERTED.[Uid] INTO @CreatedCart
    SELECT NEWID(), GETDATE(), GETDATE(), @VisitorUid, @UserUid, @UserId;
END
ELSE
    
SELECT * FROM Cart WHERE CartUid;";
            return Query<Cart>(sql,
                new {
                    UserUid = userUid.Value,
                    VisitorUid = visitorUid.Value
                }).FirstOrDefault();
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

        public Cart EditCart(Cart cart)
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
