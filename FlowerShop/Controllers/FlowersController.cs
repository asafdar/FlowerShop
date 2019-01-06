using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class FlowersController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public FlowersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        public async Task<IActionResult> Index(string category)
        {
            if (await _context.FlowerProducts.CountAsync() == 0)
            {
                List<FlowerProduct> flowers = new List<FlowerProduct>();
                flowers.Add(new FlowerProduct { Name = "Roses", Description = "Red roses.", ImagePath = "./images/roses.png", Price = 20m, DateCreated = DateTime.Now, DateLastModified = DateTime.Now });
                _context.FlowerCategories.Add(new FlowerCategory { Name = "Flowers", FlowerProducts = flowers });

                List<FlowerProduct> bouquets = new List<FlowerProduct>();
                bouquets.Add(new FlowerProduct { Name = "Christmas Bouquet", ImagePath = "/images/cbouquet.png", Description = "A festive holiday bouquet.", Price = 40m, DateCreated = DateTime.Now, DateLastModified = DateTime.Now });
                _context.FlowerCategories.Add(new FlowerCategory { Name = "Bouquets", FlowerProducts = bouquets });

                await _context.SaveChangesAsync();
            }

            ViewBag.selectedCategory = category;
            List<FlowerCategory> model;
            if (string.IsNullOrEmpty(category))
            {
                model = await this._context.FlowerCategories.Include(x => x.FlowerProducts).ToListAsync();
            }
            else
            {
                model = await this._context.FlowerCategories.Include(x => x.FlowerProducts).Where(x => x.Name == category).ToListAsync();
            }
            ViewData["Categories"] = await this._context.FlowerCategories.Select(x => x.Name).ToArrayAsync();

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            FlowerProduct model = await _context.FlowerProducts.FindAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Details(int? id, int quantity)
        {
            FlowerCart cart = null;
            if (User.Identity.IsAuthenticated)
            {
                //Authenticated path
                var currentUser = await _userManager.GetUserAsync(User);
                cart = await _context.FlowerCarts.Include(x => x.FlowerCartProducts).ThenInclude(x => x.FlowerProduct).FirstOrDefaultAsync(x => x.ApplicationUserID == currentUser.Id);
                if (cart == null)
                {
                    cart = new FlowerCart();
                    cart.ApplicationUserID = currentUser.Id;
                    cart.DateCreated = DateTime.Now;
                    cart.DateLastModified = DateTime.Now;
                    _context.FlowerCarts.Add(cart);
                }

            }
            else
            {
                if (Request.Cookies.ContainsKey("cart_id"))
                {
                    int existingCartId = int.Parse(Request.Cookies["cart_id"]);
                    cart = _context.FlowerCarts.Include(x => x.FlowerCartProducts).FirstOrDefault(x => x.ID == existingCartId);

                }
                if (cart == null)
                {
                    cart = new FlowerCart
                    {
                        DateCreated = DateTime.Now,
                    };
                    _context.FlowerCarts.Add(cart);
                }
                cart.DateLastModified = DateTime.Now;
            }

            FlowerCartProduct product = cart.FlowerCartProducts.FirstOrDefault(x => x.ID == id);
            if (product == null)
            {
                product = new FlowerCartProduct
                {
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    ID = id ?? 0,
                    Quantity = 0
                };
                cart.FlowerCartProducts.Add(product);
            }
            product.Quantity += quantity;
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

        [HttpPost]
        public async Task<IActionResult> Add(int? id, int quantity)
        {
            FlowerCart cart = null;
            if (User.Identity.IsAuthenticated)
            {
                //Authenticated path
                var currentUser = await _userManager.GetUserAsync(User);
                cart = await _context.FlowerCarts.Include(x => x.FlowerCartProducts).ThenInclude(x => x.FlowerProduct).FirstOrDefaultAsync(x => x.ApplicationUserID == currentUser.Id);
                if (cart == null)
                {
                    cart = new FlowerCart();
                    cart.ApplicationUserID = currentUser.Id;
                    cart.DateCreated = DateTime.Now;
                    cart.DateLastModified = DateTime.Now;
                    _context.FlowerCarts.Add(cart);
                }

            }
            else
            {
                if (Request.Cookies.ContainsKey("cart_id"))
                {
                    int existingCartId = int.Parse(Request.Cookies["cart_id"]);
                    cart = _context.FlowerCarts.Include(x => x.FlowerCartProducts).FirstOrDefault(x => x.ID == existingCartId);

                }
                if (cart == null)
                {
                    cart = new FlowerCart
                    {
                        DateCreated = DateTime.Now,
                    };
                    _context.FlowerCarts.Add(cart);
                }
                cart.DateLastModified = DateTime.Now;
            }

            FlowerCartProduct product = cart.FlowerCartProducts.FirstOrDefault(x => x.ID == id);
            if (product == null)
            {
                product = new FlowerCartProduct
                {
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    ID = id ?? 0,
                    Quantity = 0
                };
                cart.FlowerCartProducts.Add(product);
            }
            product.Quantity += quantity;
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