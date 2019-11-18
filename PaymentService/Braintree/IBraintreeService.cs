using Braintree;
using StoreModel.Checkout;
using StoreModel.Account;


namespace PaymentService.Braintree
{
    public interface IBraintreeService
    {
        string GenerateClientToken();
        string GeneratePaymentNonce(string token);
        Customer CreateCustomer(CustomerRequest request);
        Customer GetCustomerById(string customerId);
        Result<Transaction> Order(Checkout checkout, Users user);
    }
}
