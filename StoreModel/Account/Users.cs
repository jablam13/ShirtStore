using System;

namespace StoreModel.Account
{
    public class Users
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public int SiteId { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Active { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int LoginAttempts { get; set; }
        public Guid Validator { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ActivLastModifiedDatee { get; set; }
    }

    public class UserCredentials
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


    }
}
