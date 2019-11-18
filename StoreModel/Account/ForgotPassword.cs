using System;
using System.Collections.Generic;
using System.Text;

namespace StoreModel.Account
{
    public class ForgotPassword
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public string Email { get; set; }
        public int? Status { get; set; }
        public string StatusMessage { get; set; }
        public List<string> Errors { get; set; }
        public Dictionary<string, List<string>> FormFieldErrors { get; set; }
    }
}
