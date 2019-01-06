using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Data;
using FlowerShop.Models;

namespace FlowerShop.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
    public class FlowerShopAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlowerShopAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FlowerShopAdmin

        public async Task<IActionResult> Index()
        {
            return View(await _context.FlowerProducts.ToListAsync());
        }

        // GET: FlowerProductsAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flowerProduct = await _context.FlowerProducts
                .SingleOrDefaultAsync(m => m.ID == id);
            if (flowerProduct == null)
            {
                return NotFound();
            }

            return View(flowerProduct);
        }

        // GET: FlowerProductsAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FlowerProductsAdmin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,Price,ImagePath")] FlowerProduct flowerProduct)
        {
            if (ModelState.IsValid)
            {
                flowerProduct.DateCreated = DateTime.Now;
                flowerProduct.DateLastModified = DateTime.Now;
                _context.Add(flowerProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flowerProduct);
        }

        // GET: FlowerProductsAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flowerProduct = await _context.FlowerProducts.SingleOrDefaultAsync(m => m.ID == id);
            if (flowerProduct == null)
            {
                return NotFound();
            }
            return View(flowerProduct);
        }

        // POST: FlowerProductsAdmin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,Price,ImagePath")] FlowerProduct flowerProduct)
        {
            if (id != flowerProduct.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    flowerProduct.DateLastModified = DateTime.Now;
                    _context.Update(flowerProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlowerProductExists(flowerProduct.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(flowerProduct);
        }

        // GET: FlowerProductsAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flowerProduct = await _context.FlowerProducts
                .SingleOrDefaultAsync(m => m.ID == id);
            if (flowerProduct == null)
            {
                return NotFound();
            }

            return View(flowerProduct);
        }

        // POST: FlowerProductsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flowerProduct = await _context.FlowerProducts.SingleOrDefaultAsync(m => m.ID == id);
            _context.FlowerProducts.Remove(flowerProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlowerProductExists(int id)
        {
            return _context.FlowerProducts.Any(e => e.ID == id);
        }
    }
}