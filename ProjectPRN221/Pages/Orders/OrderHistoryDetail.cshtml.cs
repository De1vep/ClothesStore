using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using ProjectPRN221.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class OrderHistoryDetailModel : PageModel
{
    private readonly ClothesStoreContext _context;

    public OrderHistoryDetailModel(ClothesStoreContext context)
    {
        _context = context;
    }

    public List<OrderDetail> OrderItems { get; set; }
    public decimal Subtotal { get; set; }

    [BindProperty]
    public int RatingStar { get; set; }

    public string RatingReview { get; set; }

    public Order Orderr { get; set; }

    public OrderDetail od { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userIdString = HttpContext.Session.GetString("UserID");
        
        int userString = int.Parse(userIdString);

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
            return RedirectToPage("/Login");
        }

        OrderItems = await _context.OrderDetails.Include(x => x.Order).Where(x => x.Order.UserId == userString && x.OrderId == id).Include(x => x.Product).ToListAsync();

        Orderr= _context.Orders.Include(x => x.User).FirstOrDefault(x => x.UserId == userString && x.OrderId == id);

        Subtotal = OrderItems.Sum(item => item.Product.Price * item.Quantity);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync (int orderDetailID, int orderID, int userID)
    {
        if (orderDetailID == null || _context.Carts == null)
        {
            return NotFound();
        }
        var orderDetail = await _context.OrderDetails.FindAsync(orderDetailID);

        if (orderDetail != null)
        {
            od = orderDetail;
            _context.OrderDetails.Remove(od);
            await _context.SaveChangesAsync();
        }

        OrderItems = await _context.OrderDetails.Include(x => x.Order).Where(x => x.Order.UserId == userID && x.OrderId == orderID).Include(x => x.Product).ToListAsync();

        Orderr = _context.Orders.Include(x => x.User).FirstOrDefault(x => x.UserId == userID && x.OrderId == orderID);

        Subtotal = OrderItems.Sum(item => item.Product.Price * item.Quantity);

        return Page();
    }

    public async Task<IActionResult> OnPostRateAsync(int product, string rating, int ratingstar, int order)
    {
        var userIdString = HttpContext.Session.GetString("UserID");
        int userString = int.Parse(userIdString);

        // Create a new review
        Review rv = new Review
        {
            UserId = userString,
            ReviewDate = DateTime.Now,
            ReviewText = rating,
            Rating = ratingstar,
            ProductId = product
        };

        // Add and save the review
        _context.Add(rv);
        await _context.SaveChangesAsync(); // Ensure the save operation completes

        // Fetch order items and order details
        OrderItems = await _context.OrderDetails
                                    .Include(x => x.Order)
                                    .Where(x => x.Order.UserId == userString && x.OrderId == order)
                                    .Include(x => x.Product)
                                    .ToListAsync();

        Orderr = _context.Orders.Include(x => x.User).FirstOrDefault(x => x.UserId == userString && x.OrderId == order);

        // Calculate subtotal
        Subtotal = OrderItems.Sum(item => item.Product.Price * item.Quantity);

        return Page();
    }


}
