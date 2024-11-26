using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.Products
{
    public class DetailsProductToCartModel : PageModel
    {
        private readonly ProjectPRN221.Models.ClothesStoreContext _context;

        public DetailsProductToCartModel(ProjectPRN221.Models.ClothesStoreContext context)
        {
            _context = context;
        }

        public Product Product { get; set; } = default!;

        public List<Review> reviews { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p=>p.Category).FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                Product = product;
                reviews = _context.Reviews.Where(r => r.ProductId == product.ProductId).Include(r=>r.User).ToList();
            }
            return Page();
        }
    }
}
