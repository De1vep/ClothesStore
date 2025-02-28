using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;
using System.Threading.Tasks;

public class OrderConfirmationModel : PageModel
{
    private readonly ClothesStoreContext _context;

    public OrderConfirmationModel(ClothesStoreContext context)
    {
        _context = context;
    }

    public Order Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int orderId)
    {
        Order = await _context.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (Order == null)
        {
            return NotFound();
        }

        return Page();
    }
}
