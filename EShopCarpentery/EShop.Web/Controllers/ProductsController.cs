using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EShop.Web.Data;
using EShop.Web.Models.Domain;
using System.Security.Claims;

namespace EShop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductName,ProductDescription,ProductImage,MaterialType,CategoryTypes,Height,Width,Length,Price,Stock")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.Id = Guid.NewGuid();
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProductName,ProductDescription,ProductImage,MaterialType,CategoryTypes,Height,Width,Length,Price,Stock,Id")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public async Task<IActionResult> AddToCart(Guid id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(e => e.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            ProductInShoppingCart ps = new ProductInShoppingCart();
            ps.ProductId = product.Id;
            ps.Product = product;

            return View(ps);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCartConfirmed(ProductInShoppingCart ps)
        {
            if (ModelState.IsValid) 
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.EShopUsers
                   .Include(z => z.ShoppingCart)
                   .Include("ShoppingCart.ProductInShoppingCarts")
                   .Include("ShoppingCart.ProductInShoppingCarts.Product")
                   .SingleOrDefaultAsync(s => s.Id == userId);

                if (user == null)
                {
                    return NotFound();
                }

                var userShoppingCart = user.ShoppingCart;
                if (userShoppingCart.ProductInShoppingCarts == null)
                    userShoppingCart.ProductInShoppingCarts = new List<ProductInShoppingCart>();

                ps.ShoppingCartId = userShoppingCart.Id;
                userShoppingCart.ProductInShoppingCarts.Add(ps);

                _context.ShoppingCarts.Update(userShoppingCart);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction("Details", "ShoppingCarts");
        }
    }
}
