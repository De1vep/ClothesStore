using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;
using ProjectPRN221.Pages.AdminPage.Orders;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectPRN221.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly ClothesStoreContext _context;

        public CreateModel(ClothesStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Order Order { get; set; }

        [BindProperty]
        public string OrderDetails { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["page"] = "Admin";
            // Initialize the Order with default values
            Order = new Order
            {
                OrderDate = DateTime.Now,
                OrderStatus = "Pending"
            };

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
        public async Task<IActionResult> OnPostCreateOrderAsync([FromBody] OrderCreationModel model)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    _context.Orders.Update(model.Order);
                    await _context.SaveChangesAsync();

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
                            OrderId = model.Order.OrderId,
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
                    throw; // Re-throw the exception to be caught by the outer try-catch
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error creating order: {ex}");
                return new JsonResult(new { success = false, message = $"An error occurred while creating the order: {ex.Message}" });
            }
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    var orderDetails = JsonSerializer.Deserialize<List<OrderDetail>>(OrderDetails);

        //    Order.TotalAmount = orderDetails.Sum(od => od.Quantity * od.UnitPrice);
        //    _context.Orders.Add(Order);
        //    await _context.SaveChangesAsync();

        //    foreach (var orderDetail in orderDetails)
        //    {
        //        orderDetail.OrderId = Order.OrderId;
        //        _context.OrderDetails.Add(orderDetail);
        //    }

        //    await _context.SaveChangesAsync();

        //    return RedirectToPage("./Index");
        //}
    }
}
