using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShop.Models
{
    public class FlowerCart
    {
        public FlowerCart()
        {
            this.FlowerCartProducts = new HashSet<FlowerCartProduct>();
        }

        public int ID { get; set; }

        public ICollection<FlowerCartProduct> FlowerCartProducts { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateLastModified { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string ApplicationUserID { get; set; }
    }
}
