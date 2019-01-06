using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Data;
using FlowerShop.Models;

namespace FlowerShop.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            FlowerCart cart = null;
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = _userManager.GetUserAsync(User).Result;
                cart = _context.FlowerCarts.Include(x => x.FlowerCartProducts).ThenInclude(x => x.FlowerProduct).Single(x => x.ApplicationUserID == currentUser.Id);
            }
            else if (Request.Cookies.ContainsKey("cart_id"))
            {
                int existingCartID = int.Parse(Request.Cookies["cart_id"]);
                cart = _context.FlowerCarts.Include(x => x.FlowerCartProducts).ThenInclude(x => x.FlowerProduct).FirstOrDefault(x => x.ID == existingCartID);
            }
            else
            {
                cart = new FlowerCart();
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id, int quantity)
        {
            FlowerCart cart = null;
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                cart = await _context.FlowerCarts.Include(x => x.FlowerCartProducts).FirstOrDefaultAsync(x => x.ApplicationUserID == currentUser.Id);
            }
            else
            {
                if (Request.Cookies.ContainsKey("cart_id"))
                {
                    int existingCartID = int.Parse(Request.Cookies["cart_id"]);
                    cart = await _context.FlowerCarts.Include(x => x.FlowerCartProducts).FirstOrDefaultAsync(x => x.ID == existingCartID);
                    cart.DateLastModified = DateTime.Now;
                }
            }
            FlowerCartProduct product = cart.FlowerCartProducts.FirstOrDefault(x => x.ID == id);

            _context.Remove(product);

            product.Quantity -= quantity;
            product.DateLastModified = DateTime.Now;

            await _context.SaveChangesAsync();

            if (!User.Identity.IsAuthenticated)
            {
                //At the end of this page, always set the cookie.  This might just overwrite the old cookie!
                Response.Cookies.Append("cart_id", cart.ID.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1)
                });
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}