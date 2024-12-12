using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Authentication.Helper;
using Authentication.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly AuthenticationContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger, AuthenticationContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lay cart tu session
            Cart cart = Cart.Instance;
            return View(cart);
        }

        [Authorize]
        public async Task<IActionResult> Order()
        {
            // Lay cart tu session
            Cart cart = Cart.Instance;
            // Thuc hien thanh toan : Zalo, Momo
            // Luu Order vao database: Order & OrderDetail
            using (var tran = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Order order = new Order
                    {
                        Date = DateTime.Today,
                        CustomerId = null,
                        EmployeeId = null,
                    };
                    // Luu order header vao table Order
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
                    // Luu order details vao table OrderDetail
                    foreach (Item item in cart.List.Values)
                    {
                        OrderDetail orderDetail = new OrderDetail
                        {
                            OrderId = order.Id,
                            ProductId = item.Id,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Discount = item.Discount,
                        };
                        _context.OrderDetails.Add(orderDetail);
                    }
                    await _context.SaveChangesAsync();
                    // Ket thuc transaction khi thuc hien thanh cong
                    await tran.CommitAsync();
                    // Xoa cart
                    cart.Empty();
                    HttpContext.Session.Remove("cart");
                }
                catch (Exception ex)
                {
                    // Undo transaction
                    await tran.RollbackAsync();
                    _logger.LogError(ex, "Error processing order");
                    // Handle error (e.g., show error message to user)
                }
            }

            return View(cart);
        }

        public async Task<IActionResult> Add(int id)
        {
            Product? p = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (p == null)
            {
                return NotFound();
            }

            Item item = new Item
            {
                Id = p.Id,
                Category = p.Category.Name,
                Description = p.Description,
                Discount = p.Discount,
                Price = p.Price,
                Quantity = 1
            };
            // Lay cart tu session
            Cart cart = Cart.Instance;
            // Luu item vao cart
            cart.Add(item);
            // Luu cart vao session
            HttpContext.Session.Set("cart", cart);
            // Quay ve /Home/Index de hien lai home page
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Remove(int id)
        {
            // Lay cart tu session
            Cart cart = Cart.Instance;
            // Remove item tu cart
            cart.Remove(id);
            // Luu cart vao session
            HttpContext.Session.Set("cart", cart);
            // Quay ve thuc hien action /Cart/Index
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Empty()
        {
            // Lay cart tu session
            Cart cart = Cart.Instance;
            // Empty cart
            cart.Empty();
            // Luu cart vao session
            HttpContext.Session.Set("cart", cart);
            // Quay ve thuc hien action /Cart/Index
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id, int quantity)
        {
            // Lay cart tu session
            Cart cart = Cart.Instance;
            // Update cart
            cart.Update(id, quantity);
            // Luu cart vao session
            HttpContext.Session.Set("cart", cart);
            // Quay ve thuc hien action /Cart/Index
            return RedirectToAction("Index");
        }
    }
}
