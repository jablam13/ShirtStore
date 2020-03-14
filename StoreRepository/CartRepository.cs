using Microsoft.Extensions.Options;
using StoreModel.Checkout;
using StoreModel.Generic;
using StoreModel.Store;
using StoreRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreRepository
{
    public class CartRepository : BaseRepository, ICartRepository
    {
        public CartRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        { }

        public async Task<Cart> GetCartAll(Guid visitorUid, Guid userUid)
        {
            var cart = await GetCart(visitorUid, userUid).ConfigureAwait(false);
            cart.CartItems = await GetCartItems(cart.Uid).ConfigureAwait(false);

            return cart;
        }

        public async Task<Cart> GetCart(Guid visitorUid, Guid userUid)
        {
            Guid? uUid = null;
            if(userUid != Guid.Empty) { uUid = userUid; }
            const string sql = @"
IF (@VisitorUid IS NOT NULL AND @VisitorUid != '00000000-0000-0000-0000-000000000000') AND NOT EXISTS(SELECT * FROM Cart WHERE VisitorUid = @VisitorUid)
BEGIN
	INSERT INTO Cart
	SELECT NEWID(), GETDATE(),GETDATE(),@VisitorUid, @UserUid, NULL
END;

IF (@UserUid IS NOT NULL AND @UserUid != '00000000-0000-0000-0000-000000000000') AND NOT EXISTS(SELECT * FROM Cart WHERE UserUid = @UserUid)
BEGIN
	IF EXISTS(SELECT * FROM Cart WHERE VisitorUid = @VisitorUid)
	BEGIN 
		UPDATE Cart SET UserUid = @UserUid, UserId = (SELECT Id FROM SiteUser WHERE [Uid] = @UserUid AND SiteId = 1) WHERE UserUid = @UserUid;
	END
	ELSE 
	BEGIN
		INSERT INTO Cart
		SELECT NEWID(), GETDATE(),GETDATE(),@VisitorUid, @UserUid, Id FROM SiteUser WHERE [Uid] = @UserUid;
	END
END;

IF (@UserUid IS NOT NULL AND @UserUid != '00000000-0000-0000-0000-000000000000')
BEGIN
	SELECT Id, Uid, CreatedDate, LastModifiedDate, VisitorUid, UserUid, UserId FROM Cart WHERE UserUid = @UserUid;
END 
ELSE
	SELECT Id, Uid, CreatedDate, LastModifiedDate, VisitorUid, UserUid, UserId FROM Cart WHERE VisitorUid = @VisitorUid;
";
            var cart = await QueryAsync<Cart>(sql, new
            {
                VisitorUid = visitorUid,
                UserUid = uUid,
                SiteId = siteId,
            }).ConfigureAwait(false);
            return cart.FirstOrDefault();
        }

        public async Task<Cart> GetVisitorCart(Guid visitorUid)
        {
            const string sql = "SELECT Uid, CreatedDate, LastModifiedDate, VisitorUid, UserUid, UserId FROM Cart WHERE VisitorUid = @VisitorUid;";

            var cart = await QueryAsync<Cart>(sql, new
            {
                VisitorUid = visitorUid,
                SiteId = siteId,
            }).ConfigureAwait(false);
            return cart.FirstOrDefault();
        }

        public async Task<Cart> GetUserCart(Guid userUid)
        {
            const string sql = "SELECT Uid, CreatedDate, LastModifiedDate, VisitorUid, UserUid, UserId FROM Cart WHERE UserUid = @UserUid; ";

            var cart = await QueryAsync<Cart>(sql, new
            {
                UserUid = userUid,
                SiteId = siteId,
            });
            return cart.FirstOrDefault();
        }

        public async Task<List<CartItem>> GetCartItems(int cartId)
        {
            const string sql = @"
SELECT 
	ci.Id, ci.[Uid], ci.CartId AS CartId, ci.Quantity, ci.Active,
	si.Id AS ItemId, si.Uid AS ItemUid,si.[Name] AS [Name], si.[Description], si.SmallImg, si.LargeImg, si.Price
FROM CartItem ci
LEFT JOIN Cart c ON ci.CartId = c.Id
LEFT JOIN StoreItem si on si.Id = ci.ItemId 
WHERE 1=1 
c.Id = @CartId
AND ci.Active = 1; ";

            return Query<CartItem>(sql, new { CartId = cartId }).ToList();
        }

        public async Task<List<CartItem>> GetCartItems(Guid cartUid)
        {
            const string sql = @"
SELECT 
	ci.Id, ci.[Uid], ci.CartId AS CartId, ci.Quantity, ci.Active,
	si.Id AS ItemId, si.Uid AS ItemUid,si.[Name] AS [Name], si.[Description], si.SmallImg, si.LargeImg, si.Price
FROM CartItem ci
LEFT JOIN Cart c ON ci.CartId = c.Id
LEFT JOIN StoreItem si on si.Id = ci.ItemId 
WHERE 1=1 
AND c.[Uid] = @CartUid 
AND ci.Active = 1; ";

            var cartItems = await QueryAsync<CartItem>(sql, new { CartUid = cartUid });
            return cartItems.ToList();
        }

        public async Task<List<CartItem>> GetCartItems(Guid visitorUid, Guid userUid)
        {
            const string sql = @"
SELECT 
	ci.Id, ci.[Uid], ci.CartId AS CartId, ci.Quantity, ci.Active,
	si.Id AS ItemId, si.Uid AS ItemUid,si.[Name] AS [Name], si.[Description], si.SmallImg, si.LargeImg, si.Price
FROM CartItem ci
LEFT JOIN Cart c ON ci.CartId = c.Id
LEFT JOIN StoreItem si on si.Id = ci.ItemId 
WHERE 1=1 
AND (VisitorUid = @VisitorUid)
AND (c.UserUid = @UserUid OR @UserUid IS NULL OR @UserUid = '00000000-0000-0000-0000-000000000000') 
AND ci.Active = 1
AND si.Active = 1; ";

            var cartItems = await QueryAsync<CartItem>(sql, new { VisitorUid = visitorUid, UserUid = userUid });
            return cartItems.ToList();
        }

        public async Task<CartItem> AddCartItem(StoreItem item, int cartId)
        {
            const string sql = @"
DECLARE @CreatedCartItem TABLE ([Id] INT);
		
IF EXISTS(SELECT * FROM CartItem WHERE CartId = @CartId AND ItemId = @ItemId)
BEGIN 
	UPDATE CartItem 
	SET Quantity += @Quantity,
        Price = @Price,
        Active = 1,
        LastModifiedDate = GETDATE()
	OUTPUT INSERTED.Id INTO @CreatedCartItem
	WHERE CartId = @CartId AND ItemId = @ItemId
END
ELSE 
BEGIN
    INSERT INTO CartItem
	OUTPUT INSERTED.Id INTO @CreatedCartItem
    SELECT NEWID(), @CartId, @ItemId, @Quantity, GETDATE(), GETDATE(), 1, @Price
END;

SELECT 
	ci.Id, ci.[Uid], ci.CartId AS CartId, ci.Quantity, ci.Active,
	si.Id AS ItemId, si.Uid AS ItemUid,si.[Name] AS [Name], si.[Description], si.SmallImg, si.LargeImg, si.Price
FROM CartItem ci
LEFT JOIN Cart c ON ci.CartId = c.Id
LEFT JOIN StoreItem si on si.Id = ci.ItemId 
WHERE 1=1 
AND ci.Id = (SELECT TOP 1 Id FROM @CreatedCartItem)
AND ci.Active = 1
AND si.Active = 1;
";

            var cartItem = await QueryAsync<CartItem>(sql,
                new
                {
                    CartId = cartId,
                    ItemId = item.Id,
                    item.Quantity,
                    item.Price
                });
                
            return cartItem.FirstOrDefault(); ;
        }

        public async Task<CartItem> EditCartItem(CartItem cartItem)
        {
            const string sql = @"
DECLARE @EditedCartItemId TABLE ([Id] INT);

UPDATE CartItem SET Quantity = @UpdatedQuantity, Price = @Price
OUTPUT INSERTED.Id INTO @EditedCartItemId
WHERE [Uid] = @Uid;

SELECT 
	ci.Id, ci.[Uid], ci.CartId AS CartId, ci.Quantity, ci.Active,
	si.Id AS ItemId, si.Uid AS ItemUid,si.[Name] AS [Name], si.[Description], si.SmallImg, si.LargeImg, si.Price
FROM CartItem ci
INNER JOIN @EditedCartItemId eci ON eci.[Id] = ci.[Id]
LEFT JOIN Cart c ON ci.CartId = c.Id
LEFT JOIN StoreItem si on si.Id = ci.ItemId 
WHERE 1=1 
AND ci.Active = 1
AND si.Active = 1;";

            var item = await QueryAsync<CartItem>(sql, cartItem);
            return item.FirstOrDefault();
        }

        public async Task<Guid> RemoveCartItem(Guid cartItemUid)
        {
            const string sql = @"
DECLARE @RemovedCartItemUid TABLE ([Uid] UNIQUEIDENTIFIER);

UPDATE CartItem SET Active = 0, Quantity = 0, Price = 0.00
OUTPUT INSERTED.[Uid] INTO @RemovedCartItemUid
WHERE [Uid] = @CartItemUid;

SELECT TOP 1 Uid FROM @RemovedCartItemUid";

            var uid = await QueryAsync<Guid>(sql, new { CartItemUid = cartItemUid });
            return uid.FirstOrDefault();
        }

        public async Task<bool> CartExists(Guid userUid, Guid visitorUid)
        {
            const string sql = "SELECT EXIST(SELECT * FROM Cart WHERE UserUid = @UserUid OR VisitorUid = @VisitorUid);";
            var success = await QueryAsync<bool>(sql,
                new
                {
                    UserUid = userUid,
                    VisitorUid = visitorUid
                });

            return success.FirstOrDefault();
        }

        public async Task<Cart> UpdateCart(Cart cart, Guid visitorUid, Guid userUid)
        {
            const string sql = @"
             
                ";
            var updatedCart = await QueryAsync<Cart>(sql,
                new
                {
                    UserUid = userUid,
                    VisitorUid = visitorUid
                });
            return updatedCart.FirstOrDefault();
        }

        public async Task<Cart> UpdateCartAndItems(Cart userCart)
        {
            var updatedCart = await UpdateCart(userCart);

            List<Task<CartItem>> listOfTasks = new List<Task<CartItem>>();

            foreach (var item in userCart.CartItems)
            {
                listOfTasks.Add(UpdateCartItem(item));
            }

            var items = await Task.WhenAll(listOfTasks);
            updatedCart.CartItems = items.ToList();

            return updatedCart;
        }

        public async Task<Cart> UpdateCart(Cart userCart)
        {
            const string sql = @"";
            var cart = await QueryAsync<Cart>(sql, userCart);
            return cart.FirstOrDefault();
        }

        public async Task<CartItem> UpdateCartItem(CartItem userCart)
        {
            const string sql = @"";
            var cartItems = await QueryAsync<CartItem>(sql, userCart);

            return cartItems.FirstOrDefault();
        }

        public async Task<bool> DeactivateVisitorCart(Guid uid)
        {
            const string sql = @"UPDATE CartItem SET Active = 0 WHERE CartId = (SELECT Id FROM Cart WHERE Uid = @CartUid);";
            var success = await QueryAsync<bool>(sql, new { CartUid = uid });
            return success.FirstOrDefault();
        }
        public async Task<int> CartItemCount(Guid userUid)
        {
            const string sql = @"
SELECT COUNT(*) FROM Cart c 
LEFT JOIN CartItem ci ON ci.CartId = c.Id 
WHERE c.UserUid = @UserUid AND ci.Active = 1; ";
            var cartItems = await QueryAsync<int>(sql, new { UserUid = userUid });

            return cartItems.FirstOrDefault();
        }
    }
}
