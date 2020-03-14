using System;
using StoreModel.Generic;
using StoreModel.Store;
using StoreRepository.Interface;
using System.Linq;
using Microsoft.Extensions.Options;
using StoreModel.Checkout;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreRepository
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        private readonly IAddressRepository addressRep;

        public OrderRepository(IOptions<AppSettings> appSettings, IAddressRepository _addressRep) : base(appSettings)
        {
            addressRep = _addressRep;
        }


        public async Task<Order> CreateOrder(Guid userUid, string ipAddress)
        {
            const string sql = @"
DECLARE @OrderId TABLE ([Id] INT);
DECLARE @UserId INT = (SELECT Id FROM SiteUser WHERE [Uid] = @UserUid AND SiteId = @SiteId);
DECLARE @Email NVARCHAR(200) = (SELECT EmailAddress FROM SiteUser WHERE [Uid] = @UserUid AND SiteId = @SiteId);

--IF EXISTS(SELECT * FROM Orders WHERE UserId = @UserId AND OrderStateId = 1)
BEGIN
    UPDATE Orders SET OrderStateId = 4, LastModifiedDate = GETDATE() WHERE UserId = @UserId;
END;

INSERT INTO Orders
OUTPUT INSERTED.[Id] INTO @OrderId
SELECT NEWID(), @UserId, NULL, NULL, @Email, 1, GETDATE(), NULL, NULL, NULL, NULL, 0.00, NULL, 8.99, NULL, 0.00, @IPAddress, GETDATE(), GETDATE();

SELECT Id, [Uid], UserId, ShippingId, BillingId, Email, OrderStateId, OrderDate, ProcessedDate, 
    TokenId, CardAuth, Subtotal, Tax, ShippingCost, Discount, Total, IPAddress, CreatedDate, LastModifiedDate
FROM Orders WHERE Id = (SELECT Id FROM @OrderId);
";
            var success = await QueryAsync<Order>(sql, new { UserUid = userUid, SiteId = siteId, IPAddress = ipAddress}).ConfigureAwait(false);
            return success.FirstOrDefault();
        }

        public async Task<List<OrderItem>> CreateOrderItemsFromCart(Guid userUid, int orderId)
        {
            const string sql = @"
DECLARE @OrderItemIds TABLE ([Id] INT);

INSERT INTO OrderItem
OUTPUT INSERTED.[Id] INTO @OrderItemIds
SELECT NEWID(), @OrderId, ci.Id, ci.Quantity, ci.Price, ci.Price * .11, 8.99, NULL, '', 0, GETDATE()
FROM CartItem ci 
LEFT JOIN Cart c ON c.Id = ci.CartId 
WHERE 1=1 
AND c.UserUid = @UserUid 
AND ci.Active = 1 
AND ci.Quantity > 0
AND ci.Price >= 0;

SELECT Id, [Uid], OrderId, CartItemId, Quantity, Price, Tax, ShippingCost, Discount, Tracking, IsBackorder, CreatedDate 
FROM OrderItem WHERE OrderId = @OrderId;
";
            var success = await QueryAsync<OrderItem>(sql, new {
                UserUid = userUid,
                OrderId = orderId,
                SiteId = siteId,
            }).ConfigureAwait(false);
            return success.ToList();
        }

        public async Task<Order> GetOrderAll(Guid userUid, int statusId)
        {
            var order = await GetOrder(userUid, statusId).ConfigureAwait(false);

            var orderItems = GetOrderItems(order.Id);
            var shippingAddress = addressRep.GetAddressById(order.ShippingId);
            var billingAddress = addressRep.GetAddressById(order.BillingId);

            await Task.WhenAll(orderItems, shippingAddress, billingAddress);

            order.OrderItems = await orderItems;
            order.ShippingAddress = await shippingAddress;
            order.BillingAddress = await billingAddress;

            return order;
        }

        public async Task<Order> GetOrder(Guid userUid, int statusId)
        {
            const string sql = @"
DECLARE @UserId INT = (SELECT Id FROM SiteUser WHERE [Uid] = @UserUid AND SiteId = 1);
DECLARE @Email NVARCHAR(200) = (SELECT EmailAddress FROM SiteUser WHERE [Uid] = @UserUid AND SiteId = @SiteId);

SELECT Id, [Uid], UserId, ShippingId, BillingId, Email, OrderStateId, OrderDate, ProcessedDate, 
    TokenId, CardAuth, CardType, Subtotal, Tax, ShippingCost, Discount, Total, IPAddress, CreatedDate, LastModifiedDate
FROM Orders WHERE 1=1
AND UserId = @UserId
AND OrderStatusId = @OrderStatusId;
";
            var success = await QueryAsync<Order>(sql, new { UserUid = userUid, OrderStatusId = statusId, SiteId = siteId }).ConfigureAwait(false);
            return success.FirstOrDefault();
        }

        public async Task<List<OrderItem>> GetOrderItems(int orderId)
        {
            const string sql = @"
SELECT Id, [Uid], OrderId, CartItemId, Quantity, Price, Tax, ShippingCost, Discount, Tracking, IsBackorder, CreatedDate 
FROM OrderItem WHERE OrderId = @OrderId;
";
            var orderItems = await QueryAsync<OrderItem>(sql, new { OrderId = orderId, SiteId = siteId });
            return orderItems.ToList();
        }
    }
}
