using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;
using System.Threading.Tasks;

namespace ProjectPRN221.Pages.AdminPage.Orders
{
    public class DetailModel : PageModel
    {
        private readonly ClothesStoreContext _context;

        public DetailModel(ClothesStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Order Order { get; set; }

        public IEnumerable<User> Users { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["page"] = "Admin";
            if (id == null)
            {
                return NotFound();
            }

            Order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            Users = await _context.Users.ToListAsync();

            if (Order == null)
            {
                return NotFound();
            }

            return Page();
        }

        public JsonResult OnGetSearchProducts(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new JsonResult(new List<Product>());
            }

            var products = _context.Products
                .Where(p => p.ProductName.Contains(searchTerm))
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Price,
                    p.ProductImage
                })
                .ToList();

            return new JsonResult(products);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateAsync([FromBody] OrderCreationModel model)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Fetch the existing order
                    var existingOrder = await _context.Orders
                        .Include(o => o.OrderDetails)
                        .FirstOrDefaultAsync(o => o.OrderId == model.Order.OrderId);

                    if (existingOrder == null)
                    {
                        return new JsonResult(new { success = false, message = "Order not found." });
                    }

                    // Revert product quantities for existing order details
                    foreach (var existingOrderDetail in existingOrder.OrderDetails)
                    {
                        var product = await _context.Products.FindAsync(existingOrderDetail.ProductId);
                        if (product != null)
                        {
                            product.Quantity += existingOrderDetail.Quantity;
                            _context.Products.Update(product);
                        }
                    }

                    // Remove existing order details
                    _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);

                    // Update order properties
                    existingOrder.UserId = model.Order.UserId;
                    existingOrder.OrderDate = model.Order.OrderDate;
                    existingOrder.ShippingAddress = model.Order.ShippingAddress;
                    existingOrder.PhoneNumber = model.Order.PhoneNumber;
                    existingOrder.TotalAmount = model.Order.TotalAmount;
                    existingOrder.OrderStatus = model.Order.OrderStatus;

                    // Add updated order details
                    foreach (var orderDetailViewModel in model.OrderDetails)
                    {
                        var product = await _context.Products.FindAsync(orderDetailViewModel.ProductId);
                        if (product == null)
                        {
                            throw new Exception($"Product with ID {orderDetailViewModel.ProductId} not found.");
                        }

                        if (product.Quantity < orderDetailViewModel.Quantity)
                        {
                            throw new Exception($"Insufficient stock for product {product.ProductName}.");
                        }

                        var orderDetail = new OrderDetail
                        {
                            OrderId = existingOrder.OrderId,
                            ProductId = orderDetailViewModel.ProductId,
                            Quantity = orderDetailViewModel.Quantity,
                            UnitPrice = orderDetailViewModel.UnitPrice
                        };
                        _context.OrderDetails.Add(orderDetail);

                        // Subtract the quantity from the product's stock
                        product.Quantity -= orderDetailViewModel.Quantity;
                        _context.Products.Update(product);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new JsonResult(new { success = true });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the exception if needed
                    return new JsonResult(new { success = false, message = $"An error occurred while updating the order: {ex.Message}" });
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return new JsonResult(new { success = false, message = $"An error occurred while updating the order: {ex.Message}" });
            }
        }

    }
}



