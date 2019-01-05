using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShop.Models
{
    public class FlowerCategory
    {
        public FlowerCategory()
        {
            this.FlowerProducts = new HashSet<FlowerProduct>();
        }

        public string Name { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateLastModified { get; set; }

        public ICollection<FlowerProduct> FlowerProducts { get; set; }
    }
}
