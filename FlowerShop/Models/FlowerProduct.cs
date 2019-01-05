using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShop.Models
{
    public class FlowerProduct
    {
        public FlowerProduct()
        {
            this.FlowerCartProducts = new HashSet<FlowerCartProduct>();
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }

        public string ImagePath { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateLastModified { get; set; }

        public FlowerCategory FlowerCategory { get; set; }

        public string FlowerCategoryName { get; set; }

        public ICollection<FlowerCartProduct> FlowerCartProducts { get; set; }
    }
}
