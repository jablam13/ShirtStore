using System;
using System.Collections.Generic;
using System.Text;

namespace StoreModel.Account
{
    public class AuthLoginAttempt
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public Guid Uid { get; set; }
        public string EmailAddress { get; set; }
    }
}
