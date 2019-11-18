using System;
using System.Collections.Generic;
using System.Text;

namespace StoreModel.Generic
{
    public class Error
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
