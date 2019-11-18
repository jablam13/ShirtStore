using System;
using System.Collections.Generic;
using System.Text;

namespace StoreModel.Checkout
{
    public class OrderResult
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }
        public Dictionary<string, List<string>> MultiErrors { get; set; }
    }
}
