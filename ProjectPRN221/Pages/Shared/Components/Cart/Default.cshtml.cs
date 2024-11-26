using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;
using System.Linq;
using System.Threading.Tasks;

public class CartViewComponent : ViewComponent
{
    private readonly ClothesStoreContext _context;

    public CartViewComponent(ClothesStoreContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userIdString = HttpContext.Session.GetString("UserID");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            // Nếu người dùng chưa đăng nhập, không hiển thị giỏ hàng
            return View(new List<Cart>());
        }

        var cartItems = await _context.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ToListAsync();

        return View(cartItems);
    }
}
