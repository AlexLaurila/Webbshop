using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WoM_Labb1.Data;
using WoM_Labb1.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace WoM_Labb1.Controllers
{
    public class ProduktsController : Controller
    {
        private readonly ProduktContext _context;

        public ProduktsController(ProduktContext context)
        {
            _context = context;
        }

        // GET: Produkts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Produkt.ToListAsync());
        }

        // GET: Produkts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produkt = await _context.Produkt
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produkt == null)
            {
                return NotFound();
            }

            return View(produkt);
        }

        // GET: Produkts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produkts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,productName,productDescription,productPrice,productInStore")] Produkt produkt, IFormFile productImage)
        {
            if (ModelState.IsValid)
            {
                if (productImage != null)
                {
                    byte[] bytes;
                    using (BinaryReader reader = new BinaryReader(productImage.OpenReadStream()))
                    {
                        bytes = reader.ReadBytes(Convert.ToInt32(productImage.Length));
                    }
                    produkt.productImage = bytes;
                }

                _context.Add(produkt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produkt);
        }

        // GET: Produkts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produkt = await _context.Produkt.FindAsync(id);
            if (produkt == null)
            {
                return NotFound();
            }
            return View(produkt);
        }

        // POST: Produkts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,productName,productDescription,productPrice,productInStore")] Produkt produkt, IFormFile productImage)
        {
            if (id != produkt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (productImage != null)
                {
                    byte[] bytes;
                    using (BinaryReader reader = new BinaryReader(productImage.OpenReadStream()))
                    {
                        bytes = reader.ReadBytes(Convert.ToInt32(productImage.Length));
                    }
                    produkt.productImage = bytes;
                }

                try
                {
                    _context.Update(produkt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProduktExists(produkt.Id))
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
            return View(produkt);
        }

        // GET: Produkts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produkt = await _context.Produkt
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produkt == null)
            {
                return NotFound();
            }

            return View(produkt);
        }

        // POST: Produkts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produkt = await _context.Produkt.FindAsync(id);
            _context.Produkt.Remove(produkt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProduktExists(int id)
        {
            return _context.Produkt.Any(e => e.Id == id);
        }
    }
}
