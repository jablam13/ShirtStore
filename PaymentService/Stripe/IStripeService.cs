using StoreModel.Checkout;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Stripe
{
    public interface IStripeService
    {
        void TestMethod();
        Task<string> CreateSessionTest();
        Task<string> CreateSession(List<OrderItem> orderItems);
    }
}
