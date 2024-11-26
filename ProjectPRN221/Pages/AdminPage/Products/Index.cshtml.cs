using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.AdminPage.Products
{
    public class IndexModel : PageModel
    {
        private readonly ClothesStoreContext _context;

        public IndexModel(ClothesStoreContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? ProductName { get; set; }


        [BindProperty(SupportsGet = true)]
        public int? MinUnitPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxUnitPrice { get; set; }

        public IList<Product> Product { get; set; } = default!;

        public async Task OnGetAsync()
        {
            ViewData["page"] = "Admin";
            if (_context.Products != null)
            {
                Product = await _context.Products
                .Include(p => p.Category).OrderByDescending(p => p.ProductId).ToListAsync();
            }
            var productsQuery = _context.Products.Include(p => p.Category).OrderByDescending(p => p.ProductId).AsQueryable();
            if (!string.IsNullOrEmpty(ProductName))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(ProductName));
            }

            if (MinUnitPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= MinUnitPrice.Value);
            }

            if (MaxUnitPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= MaxUnitPrice.Value);
            }

            Product = await productsQuery.ToListAsync();
        }
    }
}
