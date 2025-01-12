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
using EShop.Web.Models.DTO;
using Microsoft.CodeAnalysis;
using System.Text;
using EShop.Web.Models.Email;

namespace EShop.Web.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: ShoppingCarts/Details/5
        public async Task<IActionResult> Details()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var sc = await _context.ShoppingCarts
                .Include(s => s.Owner)
                .Include(s => s.ProductInShoppingCarts)
                .Include("ProductInShoppingCarts.Product")
                .SingleOrDefaultAsync(s => s.OwnerId == userId);
            if(sc == null)
            {
                return NotFound();
            }

            if(sc.ProductInShoppingCarts == null)
            {
                sc.ProductInShoppingCarts = new List<ProductInShoppingCart>();
            }

            float total = 0.0f;
            foreach(var ps in sc.ProductInShoppingCarts)
            {
                total += ps.Product.Price * ps.Quantity;
            }

            ShoppingCartDto dto = new ShoppingCartDto
            {
                ProductsInShoppingCart = sc.ProductInShoppingCarts.ToList(),
                Total = total,
            };

            return View(dto);
        }

        public async Task<IActionResult> DeleteFromShoppingCart(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) {
                return NotFound();
            }

            var userShoppingCart = await _context.ShoppingCarts
                .Include(s => s.Owner)
                .Include(s => s.ProductInShoppingCarts)
                .Include("ProductInShoppingCarts.Product")
                .SingleOrDefaultAsync(s => s.OwnerId == userId);

            var productInShoppingCart = userShoppingCart?.ProductInShoppingCarts?.Where(x => x.Id == id).FirstOrDefault();

            userShoppingCart?.ProductInShoppingCarts?.Remove(productInShoppingCart!);

            _context.Update(userShoppingCart!);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details");
        }


    }
}
