using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class OrderListModel : PageModel
{
    private readonly ClothesStoreContext _context;

    public OrderListModel(ClothesStoreContext context)
    {
        _context = context;
    }

    public List<Order> OrderList { get; set; }
    public decimal Subtotal { get; set; }

    public async Task<IActionResult> OnGetAsync(int status)
    {
        ViewData["Status"] = status;

        var userIdString = HttpContext.Session.GetString("UserID");

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
            return RedirectToPage("/Login");
        }

        IQueryable<Order> query = _context.Orders.Where(o => o.UserId == userId)
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Product);

        switch (status)
        {
            case 1:
                query = query.Where(o => o.OrderStatus.Equals("Pending"));
                break;
            case 2:
                query = query.Where(o => o.OrderStatus.Equals("Shipped"));
                break;
            case 3:
                query = query.Where(o => o.OrderStatus.Equals("Delivered"));
                break;
        }

        OrderList = await query.ToListAsync();  
        return Page();
    }

    public IActionResult OnPostDetail(int id)
    {
        return RedirectToPage("/Orders/OrderHistoryDetail", new { id });
    }

    public async Task<IActionResult> OnPostEditAsync(string address, string phone, int orderID, int userID)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderID && x.UserId == userID);
        if (order != null)
        {
            order.ShippingAddress = address;
            order.PhoneNumber = phone;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        OrderList = await _context.Orders.Where(o => o.UserId == userID)
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Product)
            .ToListAsync();

        return RedirectToPage();
    }
}

