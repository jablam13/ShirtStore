using System;
using System.Collections.Generic;
using System.Text;

namespace StoreModel.Account
{
    public class UserAddress
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string Street1 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string ZipCode { get; set; }
        public bool IsBilling { get; set; }
        public int Active { get; set; }
        public int PrimaryAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
