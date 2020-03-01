using System;
using System.Collections.Generic;
using System.Text;

namespace StoreModel.Generic
{
    public class AppSettings
    {
        public BaseUrls BaseUrls { get; set; }
        public string ConnectionString { get; set; }
        public bool AnalyticsEnabled { get; set; }
        public int SiteId { get; set; }
        public SendGridSettings SendGridSetting { get; set; }
        public BraintreeSettings BraintreeSettings { get; set; }
        public StripeSettings StripeSettings { get; set; }
    }

    public class BaseUrls
    {
        public string Api { get; set; }
        public string Auth { get; set; }
        public string Web { get; set; }
    }

    public class SendGridSettings
    {
        public string Host { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public bool EnableSsl { get; set; }
    }

    public class BraintreeSettings
    {
        public string Environment { get; set; }
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }

    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}
