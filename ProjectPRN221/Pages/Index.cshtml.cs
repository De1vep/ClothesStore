using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPRN221.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ProjectPRN221.Models.ClothesStoreContext _context;

        public IndexModel(ProjectPRN221.Models.ClothesStoreContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Product> Product { get; set; } = default!;
        public IList<Category> Categories { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Products != null)
            {
                Product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                    .ToListAsync();
            }

            if (_context.Categories != null)
            {
                Categories = await _context.Categories.ToListAsync();
            }
        }

        public async Task<JsonResult> OnGetSearchProductsAsync(string searchQuery, int? categoryId)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                products = products.Where(p => p.ProductName.Contains(searchQuery));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            var result = await products.Select(p => new {
                productId = p.ProductId,
                productName = p.ProductName,
                productImage = p.ProductImage,
                price = p.Price
            }).ToListAsync();

            return new JsonResult(result);
        }
    }
}
