using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShop.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base()
        {
            this.FlowerCart = new FlowerCart();
        }

        public ApplicationUser(string userName) : base(userName)
        {
            this.FlowerCart = new FlowerCart();
        }

        public FlowerCart FlowerCart { get; set; }

        public int FlowerCartID { get; set; }
    }
}
