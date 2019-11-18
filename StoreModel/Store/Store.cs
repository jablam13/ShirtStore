using System;
using System.Collections.Generic;
using System.Text;

namespace StoreModel.Store
{
    public class Store
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LargeImg { get; set; }
        public string SmallImg { get; set; }
        public int Active { get; set; }
        public List<StoreItem> StoreItems { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
