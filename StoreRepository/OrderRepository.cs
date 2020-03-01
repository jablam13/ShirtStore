using System;
using StoreModel.Generic;
using StoreModel.Store;
using StoreRepository.Interface;
using System.Linq;
using Microsoft.Extensions.Options;
using StoreModel.Checkout;
using System.Collections.Generic;

namespace StoreRepository
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        { }

        public Order CreateOrder(Order order)
        {
            const string sql = @"
DECLARE @InsertedOrder TABLE ([Id] INT);

INSERT INTO Orders
OUTPUT INSERTED.Id INTO @InsertedOrder
SELECT NEWID(), @UserId, @ShippingId, @BillingId, @Email, 1, GETDATE(), NULL, @TokenId, @CardAuth, @CardType, @SubTotal, @Tax, @ShippingCost, @Discount, @Total, @IpAddress;

SELECT TOP 1 * FROM Orders WHERE Id = (SELECT TOP 1 Id FROM @InsertedOrder);
";

            var o = Query<Order>(sql, order).FirstOrDefault();

            order.OrderItems.Select(item => { item.OrderId = o.Id; return item; }).ToList();
            o.OrderItems = CreateOrderItems(order.OrderItems);

            return o;
        }

        public List<OrderItem> CreateOrderItems(List<OrderItem> items)
        {
            const string sql = @"
DECLARE @InsertedOrderItem TABLE ([Id] INT);
DECLARE @StoreItemInfoId INT = (SELECT Id FROM StoreItemInfo WHERE Uid = @StoreItemInfoUid);

INSERT INTO OrderItem
OUTPUT INSERTED.Id INTO @InsertedOrderItem
SELECT NEWID(), @OrderId, @StoreItemInfoId, @Quantity, @Price, @Tax, @ShippingCost, @Discount, @Tracking, 0;

SELECT TOP 1 * FROM OrderItem WHERE Id = (SELECT TOP 1 Id FROM @InsertedOrderItem)";

            var itemList = new List<OrderItem>();

            foreach (var o in items)
            {
                var itemAdded = Query<OrderItem>(sql, new
                {
                    o.OrderId,
                    o.StoreItemInfoUid,
                    o.Quantity,
                    o.Price,
                    o.Tax,
                    o.ShippingCost,
                    o.Discount,
                    o.Tracking
                }).FirstOrDefault();
                itemList.Add(itemAdded);
            }

            return itemList ?? new List<OrderItem>();
        }

        public void ProcessOrder(Order order)
        {
            const string sql = @"
UPDATE Orders SET OrderStateId = 2, CardType = @CardType, TokenId = @TokenId, ProcessedDate = GETDATE()  WHERE Id = @OrderId;
";
            Execute(sql, order);

            const string sqlItems = @"
DECLARE @UserUid INT = (SELECT Uid FROM UserDetails WHERE Id = @UserId);
DECLARE @CartId INT = (SELECT Id FROM Cart WHERE UserUid = @UserUid );
DECLARE @StoreItemInfoId INT = (SELECT Id FROM StoreItemInfo WHERE Uid = @StoreItemInfoUid);
DECLARE @UpdatedItems TABLE (Id INT, Quantity INT);

UPDATE CartItem SET Quantity = 0 
OUTPUT INSERTED.Id AS Id, INSERTED.Quantity AS Quantity INTO @UpdatedItems(Id, Quantity)
WHERE CartId = @CartId AND ItemId = @StoreItemInfoId
			
INSERT INTO CartLog 
SELECT NEWID(), ici.Id, -ici.Quantity, sii.Price, 6,GETDATE()
FROM CartItem ci JOIN @UpdatedItems ici ON ici.Id = ci.Id 
LEFT JOIN StoreItemInfo sii on sii.Id = ci.ItemId 
WHERE ici.Quantity > 0;";

            //foreach (var item in order.OrderItems)
            //{
            //    DynamicParameters parameter = new DynamicParameters();

            //    Execute(sqlItems, new
            //    {
            //        OrderId = order.Id,
            //        StoreItemUid = item.StoreItemInfoUid,
            //        Quantity = item.Quantity,
            //        UserId = order.UserId
            //    });
            //}
        }
    }
}
