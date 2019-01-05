using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShop.Models
{
    public class FlowerCartProduct
    {
        public int ID { get; set; }

        public FlowerCart FlowerCart { get; set; }

        public int FlowerCartID { get; set; }

        public FlowerProduct FlowerProduct { get; set; }

        public int FlowerProductID { get; set; }

        public int? Quantity { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateLastModified { get; set; }
    }
}
