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
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _context.Orders.Where(o => o.OwnerId == userId).ToListAsync();
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Address,ContactPhone,Price,AdditionalInformation")] Order order)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    order.Id = Guid.NewGuid();
                    order.Status = Models.Enums.OrderStatus.Created;
                    order.OwnerId = userId;

                    if (order.ProductsInOrder == null)
                        order.ProductsInOrder = new List<ProductInOrder>();

                    var userShoppingCart = await _context.ShoppingCarts
                        .Include(s => s.Owner)
                        .Include(s => s.ProductInShoppingCarts)
                        .Include("ProductInShoppingCarts.Product")
                        .SingleOrDefaultAsync(s => s.OwnerId == userId);

                    float total = 0;

                    foreach (var product in userShoppingCart!.ProductInShoppingCarts!)
                    {
                        ProductInOrder productInOrder = new ProductInOrder
                        {
                            Id = new Guid(),
                            ProductId = product.ProductId,
                            OrderId = order.Id,
                            Quantity = product.Quantity
                        };

                        total += product.Product!.Price * product.Quantity;
                        order.ProductsInOrder.Add(productInOrder);
                    }

                    var supplier = await _context.Suppliers.FirstAsync();
                    order.supplierId = supplier.Id;

                    order.Price = total + supplier.PricePerKm * 5; //5 defoltna rednost za protopito (se presmetuva rastojanie megju adresata na kupuvacot i dostavuvacot)

                    userShoppingCart.ProductInShoppingCarts = new List<ProductInShoppingCart>();

                    _context.Add(order);
                    _context.Update(userShoppingCart);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Payment", "Payments", new { orderId = order.Id });
                }
            }
            return View(order);
        }

    }
}
