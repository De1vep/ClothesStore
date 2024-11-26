using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CheckoutModel : PageModel
{
    private readonly ClothesStoreContext _context;

    public CheckoutModel(ClothesStoreContext context)
    {
        _context = context;
    }

    public List<Cart> CartItems { get; set; }
    public decimal Subtotal { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdString = HttpContext.Session.GetString("UserID");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            // If the user is not logged in, redirect to the login page
            return RedirectToPage("/Account/Login");
        }

        CartItems = await _context.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ToListAsync();

        Subtotal = CartItems.Sum(item => item.Product.Price * item.Quantity);

        return Page();
    }

    public async Task<IActionResult> OnPostCheckoutAsync(string PhoneNumber, string ShippingAddress)
    {
        var userIdString = HttpContext.Session.GetString("UserID");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            // If the user is not logged in, redirect to the login page
            return RedirectToPage("/Account/Login");
        }

        var cartItems = await _context.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ToListAsync();

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            ShippingAddress = ShippingAddress,
            PhoneNumber = PhoneNumber,
            TotalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity) + 10, // Add shipping fee
            OrderStatus = "Pending"
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in cartItems)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price
            };
            _context.OrderDetails.Add(orderDetail);
            _context.Carts.Remove(item);
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("/Orders/Confirmation", new { orderId = order.OrderId });
    }

    public async Task<IActionResult> OnPostUpdateQuantityAsync(int cartId, string action)
    {
        var cartItem = await _context.Carts.FindAsync(cartId);
        if (cartItem == null)
        {
            return new JsonResult(new { success = false, message = "Cart item not found" });
        }

        if (action == "increase")
        {
            cartItem.Quantity++;
        }
        else if (action == "decrease" && cartItem.Quantity > 1)
        {
            cartItem.Quantity--;
        }

        await _context.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostRemoveItemAsync(int cartId)
    {
        var cartItem = await _context.Carts.FindAsync(cartId);
        if (cartItem == null)
        {
            return new JsonResult(new { success = false, message = "Cart item not found" });
        }

        _context.Carts.Remove(cartItem);
        await _context.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }
}
