using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Data;
using FlowerShop.Models;

namespace FlowerShop.Controllers
{
    public class ReceiptController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReceiptController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Receipt/Details/5
        public async Task<IActionResult> Index(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flowerOrder = await _context.FlowerOrders.Include(x => x.FlowerOrderProducts)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (flowerOrder == null)
            {
                return NotFound();
            }

            return View(flowerOrder);
        }
    }
}