using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShop.Models
{
    public class CheckoutModel
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,3}$")]
        [Display(Name = "Email Address")]
        [MinLength(10)]
        public string Email { get; set; }


        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        [Required]
        public string Nonce { get; set; }
    }
}
