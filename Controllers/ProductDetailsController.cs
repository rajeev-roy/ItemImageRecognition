using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FindProductByImage.Models;
using FindProductByImage.ef;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace FindProductByImage.Controllers
{
    public class ProductDetailsController : Controller
    {
        private readonly DataContext _context;

        public ProductDetailsController(DataContext context)
        {
            _context = context;
        }

        // GET: ProductDetails
        public async Task<IActionResult> Index(int page = 1)
        {
            var query = _context.ProductDetails.AsNoTracking().OrderBy(s => s.ID);
            var model = await PagingList.CreateAsync(query, 5, page);
            return View(model);
        }

        // GET: ProductDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDetails = await _context.ProductDetails
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productDetails == null)
            {
                return NotFound();
            }

            return View(productDetails);
        }

        // GET: ProductDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Price,Description,Datetime")] ProductDetails productDetails)
        {
            if (ModelState.IsValid)
            {
                //var obj = _context.ProductDetails.OrderByDescending(S => S.ID).FirstOrDefault();
                //var temp = obj?.ID + 1;
                productDetails.Datetime = DateTime.Now;
                _context.Add(productDetails);
                await _context.SaveChangesAsync();
                int tempID = productDetails.ID;
                //return RedirectToAction("camera","Cam",new {id=temp});
                TempData["idData"] = tempID;
                //ViewBag["idData1"] = tempID;
                HttpContext.Session.SetString("idval", tempID.ToString());

                return RedirectToAction("Capture", "Camera", new { id = tempID });
                //return RedirectToAction("Capture", "Camera", new { id = productDetails });
            }
            else
                return View(productDetails);
        }

        // GET: ProductDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDetails = await _context.ProductDetails.FindAsync(id);
            if (productDetails == null)
            {
                return NotFound();
            }
            return View(productDetails);
        }

        // POST: ProductDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Price,Description,Datetime")] ProductDetails productDetails)
        {
            if (id != productDetails.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    productDetails.Datetime = DateTime.Now;
                    _context.Update(productDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductDetailsExists(productDetails.ID))
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
            return View(productDetails);
        }

        // GET: ProductDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDetails = await _context.ProductDetails
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productDetails == null)
            {
                return NotFound();
            }

            return View(productDetails);
        }

        // POST: ProductDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productDetails = await _context.ProductDetails.FindAsync(id);
            _context.ProductDetails.Remove(productDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductDetailsExists(int id)
        {
            return _context.ProductDetails.Any(e => e.ID == id);
        }
    }
}
