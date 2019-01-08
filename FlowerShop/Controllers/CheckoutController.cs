using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.Services;
using SmartyStreets.USStreetApi;

namespace FlowerShop.Controllers
{
    public class CheckoutController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IEmailSender _emailSender;
        private IBraintreeGateway _braintreeGateway;
        private Client _client;

        public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IBraintreeGateway braintreeGateway, Client client)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _braintreeGateway = braintreeGateway;
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            CheckoutModel model = new CheckoutModel();
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                model.Email = currentUser.Email;
            }

            ViewBag.ClientAuthorization = await _braintreeGateway.ClientToken.GenerateAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutModel model, string nonce)
        {
            if (ModelState.IsValid)
            {
                FlowerOrder order = new FlowerOrder
                {
                    City = model.City,
                    State = model.State,
                    Email = model.Email,
                    StreetAddress = model.StreetAddress,
                    ZipCode = model.ZipCode,
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now
                };
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
                foreach (var cartItem in cart.FlowerCartProducts)
                {
                    order.FlowerOrderProducts.Add(new FlowerOrderProduct
                    {
                        DateCreated = DateTime.Now,
                        DateLastModified = DateTime.Now,
                        Quantity = cartItem.Quantity ?? 1,
                        ProductID = cartItem.ID,
                        ProductDescription = cartItem.FlowerProduct.Description,
                        ProductName = cartItem.FlowerProduct.Name,
                        ProductPrice = cartItem.FlowerProduct.Price ?? 0
                    });
                }

                _context.FlowerCartProducts.RemoveRange(cart.FlowerCartProducts);
                _context.FlowerCarts.Remove(cart);

                if (Request.Cookies.ContainsKey("cart_id"))
                {
                    Response.Cookies.Delete("cart_id");
                }

                _context.FlowerOrders.Add(order);
                _context.SaveChanges();

                //await _braintreeGateway.Transaction.SaleAsync(new TransactionRequest
                //{
                //    Amount = (decimal)order.FlowerOrderProducts.Sum(x => x.Quantity * x.ProductPrice),    //You can also do 1m here
                //    CreditCard = new TransactionCreditCardRequest
                //    {
                //        CardholderName = "Test Cardholder",
                //        CVV = "123",
                //        ExpirationMonth = DateTime.Now.AddMonths(1).ToString("MM"),
                //        ExpirationYear = DateTime.Now.AddMonths(1).ToString("yyyy"),
                //        Number = "4111111111111111"
                //    }
                //});
                Customer c = null;
                var csr = new CustomerSearchRequest();
                csr.Email.Is(model.Email);
                var customerSearchResult = await _braintreeGateway.Customer.SearchAsync(csr);
                if (customerSearchResult.Ids.Any())
                {
                    c = customerSearchResult.FirstItem;
                }
                else
                {
                    var cusResult = await _braintreeGateway.Customer.CreateAsync(new CustomerRequest { Email = model.Email });
                    c = cusResult.Target;
                }



                var card = await _braintreeGateway.PaymentMethod.CreateAsync(new PaymentMethodRequest { PaymentMethodNonce = nonce, CustomerId = c.Id });
                
               
                if (card.IsSuccess())
                {
                    
                        var result = await _braintreeGateway.Transaction.SaleAsync(new TransactionRequest
                        {
                            Amount = (decimal)order.FlowerOrderProducts.Sum(x => x.Quantity * x.ProductPrice),
                            PaymentMethodToken = card.Target.Token

                        });

                        await _emailSender.SendEmailAsync(model.Email, "Your order " + order.ID, "Thanks for ordering!  You bought : " + String.Join(",", order.FlowerOrderProducts.Select(x => x.ProductName)));

                        //TODO: Save this information to the database so we can ship the order
                        return RedirectToAction("Index", "Receipt", new { id = order.ID });
                 
                }
                else
                {
                    ModelState.AddModelError("CreditCard", "Problem with credit card");
                }


            }
            //TODO: we have an error!  Redisplay the form!
            return View();
        }

        [HttpPost]
        public IActionResult ValidateAddress([FromBody]Lookup lookup)
        {
            try
            {
                _client.Send(lookup);
                if (lookup.Result.Any())
                {
                    return Json(lookup.Result.First());
                }
                else
                {
                    return BadRequest("No matches found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}