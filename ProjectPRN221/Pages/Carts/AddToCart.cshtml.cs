using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.Carts
{
    public class AddToCartModel : PageModel
    {
        private readonly ProjectPRN221.Models.ClothesStoreContext _context;

        public AddToCartModel(ProjectPRN221.Models.ClothesStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int productId)
        {
            var userIdString = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToPage("/Account/Login");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            if (cartItem == null)
            {
                // Nếu chưa có, thêm sản phẩm vào giỏ hàng
                cartItem = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1
                };
                _context.Carts.Add(cartItem);
            }
            else
            {
                // Nếu đã có, tăng số lượng sản phẩm
                cartItem.Quantity++;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
