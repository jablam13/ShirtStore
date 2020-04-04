using Braintree;
using StoreModel.Account;
using StoreModel.Generic;
using StoreModel.Checkout;
using Microsoft.Extensions.Options;

namespace PaymentService.Braintree
{
    public class BraintreeService : IBraintreeService
    {
        private readonly BraintreeSettings btSettings;
        private readonly SendGridSettings sdSettings;
        private IBraintreeGateway gateway { get; set; }

        public BraintreeService(IOptions<AppSettings> _appSettings)
        {
            btSettings = _appSettings.Value.BraintreeSettings;
            sdSettings = _appSettings.Value.SendGridSetting;
            gateway = CreateGateway();
        }

        public static readonly TransactionStatus[] transactionSuccessStatuses = {
                TransactionStatus.AUTHORIZED,
                TransactionStatus.AUTHORIZING,
                TransactionStatus.SETTLED,
                TransactionStatus.SETTLING,
                TransactionStatus.SETTLEMENT_CONFIRMED,
                TransactionStatus.SETTLEMENT_PENDING,
                TransactionStatus.SUBMITTED_FOR_SETTLEMENT
        };

        private IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(
                btSettings.Environment, 
                btSettings.MerchantId, 
                btSettings.PublicKey, 
                btSettings.PrivateKey);
        }

        private IBraintreeGateway GetGateway()
        {
            if (gateway == null)
            {
                gateway = CreateGateway();
            }

            return gateway;
        }

        public string GenerateClientToken()
        {
            if (gateway == null) { GetGateway(); }
            return gateway.ClientToken.Generate();
        }

        public string GeneratePaymentNonce(string token)
        {
            if (gateway == null) { GetGateway(); }
            Result<PaymentMethodNonce> result = gateway.PaymentMethodNonce.Create(token);
            return result.Target.Nonce;
        }

        public Customer CreateCustomer(CustomerRequest request)
        {
            if (gateway == null) { GetGateway(); }
            Result<Customer> result = gateway.Customer.Create(request);
            
            bool success = result.IsSuccess();

            string customerId = result.Target.Id;

            return result.Target;
        }

        public Customer GetCustomerById(string customerId)
        {
            if (gateway == null) { GetGateway(); }
            var customer = gateway.Customer.Find(customerId);
            return customer;
        }


        public Result<Transaction> Order(Checkout checkout, Users user)
        {
            var customerRequest = InitCustomerRequest(user);
            var shippingRequest = InitShippingAddressRequest(checkout);
            var billingRequest = InitBillingAddressRequest(checkout);
            var transactionRequest = InitTransactionRequest(checkout, customerRequest, shippingRequest, billingRequest);

            return ProcessTransaction(transactionRequest);
        }

        private Result<Transaction> ProcessTransaction(TransactionRequest transactionRequest)
        {
            if (gateway == null) { GetGateway(); }

            Result<Transaction> result = gateway.Transaction.Sale(transactionRequest);

            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;
                //Console.WriteLine("Success!: " + transaction.Id);
            }
            else if (result.Transaction != null)
            {
                Transaction transaction = result.Transaction;
                //Console.WriteLine("Error processing transaction:");
                //Console.WriteLine("  Status: " + transaction.Status);
                //Console.WriteLine("  Code: " + transaction.ProcessorResponseCode);
                //Console.WriteLine("  Text: " + transaction.ProcessorResponseText);
            }
            else
            {
                //foreach (ValidationError error in result.Errors.DeepAll())
                //{
                //    Console.WriteLine("Attribute: " + error.Attribute);
                //    Console.WriteLine("  Code: " + error.Code);
                //    Console.WriteLine("  Message: " + error.Message);
                //}
            }

            return result;
        }

        private CustomerRequest InitCustomerRequest(Users user)
        {

            var custRequest = new CustomerRequest
            {
                Id = user.Uid.ToString(),
                FirstName = user.FirstName ?? "",
                LastName = user.FirstName ?? "",
                Email = user.EmailAddress ?? "",
                //Phone = model.PhoneNumber
            };

            return custRequest;
        }

        private AddressRequest InitShippingAddressRequest(Checkout checkout)
        {
            var addressRequest = new AddressRequest
            {
                CountryCodeAlpha2 = "US",
                PostalCode = checkout.BillingAddress?.ZipCode,
                LastName = checkout.BillingAddress?.LastName ?? checkout.ShippingAddress?.LastName,
                FirstName = checkout.BillingAddress?.FirstName ?? checkout.ShippingAddress?.FirstName,
            };

            return addressRequest;
        }

        private AddressRequest InitBillingAddressRequest(Checkout checkout)
        {
            var addressRequest = new AddressRequest
            {
                CountryCodeAlpha2 = "US",
                PostalCode = checkout.ShippingAddress?.ZipCode,
                Region = checkout.ShippingAddress?.StateCode,
                Locality = checkout.ShippingAddress?.City,
                ExtendedAddress = checkout.ShippingAddress?.Street2,
                StreetAddress = checkout.ShippingAddress?.Street,
                LastName = checkout.ShippingAddress?.LastName,
                FirstName = checkout.ShippingAddress?.FirstName,
            };

            return addressRequest;
        }
        private TransactionRequest InitTransactionRequest(
            Checkout checkout, 
            CustomerRequest user,
            AddressRequest shippingAddress,
            AddressRequest billingAddress)
        {
            var transactionRequest = new TransactionRequest()
            {
                Customer = user,
                BillingAddress = billingAddress,
                ShippingAddress = shippingAddress,
                //Amount = checkout.Amount,
                //PaymentMethodNonce = checkout.BraintreeNonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true,
                    StoreInVault = true
                }
            };

            return transactionRequest;
        }
    }
}
