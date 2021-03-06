﻿using System;
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

--insert order items from cart
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

--update orders totals from order items
;WITH SumOrderItems AS (
	SELECT OrderId, SUM(ISNULL(Price, 0)) AS Subtotal, SUM(ISNULL(Tax, 0)) AS Tax, SUM(ISNULL(Discount, 0)) AS Discount FROM OrderItem WHERE OrderId = @OrderId GROUP BY OrderId 
)
UPDATE
    Orders
SET
    Subtotal = soi.Subtotal, Tax = soi.Tax, Discount = soi.Discount, Total = soi.Subtotal + soi.Tax + o.ShippingCost - soi.Discount
FROM
    Orders o
INNER JOIN
    SumOrderItems soi
ON 
    o.Id = soi.OrderId;

--return order items
SELECT oi.Id, oi.[Uid], oi.OrderId, oi.CartItemId, oi.Quantity, oi.Price, oi.Tax, oi.ShippingCost, oi.Discount, oi.Tracking, oi.IsBackorder, oi.CreatedDate,
    si.Name, si.Description, si.SmallImg
FROM OrderItem oi
JOIN CartItem ci ON ci.Id = oi.CartItemId
JOIN StoreItem si ON si.Id = ci.ItemId
WHERE OrderId = @OrderId;
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
        public async Task<Order> InitProcessOrder(Checkout checkout, Guid userUid)
        {
            const string sql = @"
SELECT Id, [Uid], OrderId, CartItemId, Quantity, Price, Tax, ShippingCost, Discount, Tracking, IsBackorder, CreatedDate 
FROM OrderItem WHERE OrderId = @OrderId;
";
            var order = await QueryAsync<Order>(sql, new { SiteId = siteId }).ConfigureAwait(false);
            return order.FirstOrDefault();
        }
    }
}
