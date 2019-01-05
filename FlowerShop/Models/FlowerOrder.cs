using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShop.Models
{
    public class FlowerOrder
    {
        public FlowerOrder()
        {
            this.FlowerOrderProducts = new HashSet<FlowerOrderProduct>();
        }

        public Guid ID { get; set; }

        public string Email { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateLastModified { get; set; }

        public ICollection<FlowerOrderProduct> FlowerOrderProducts { get; set; }
    }
}
