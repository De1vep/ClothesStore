using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.AdminPage.Products
{
    public class DeleteModel : PageModel
    {
        private readonly ProjectPRN221.Models.ClothesStoreContext _context;

        public DeleteModel(ProjectPRN221.Models.ClothesStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["page"] = "Admin";
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                Product = product;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            // Load the product with related reviews
            var product = await _context.Products
                .Include(p => p.Reviews) // Include reviews
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product != null)
            {
                // Remove all associated reviews
                _context.Reviews.RemoveRange(product.Reviews);

                // Remove the product
                _context.Products.Remove(product);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

    }
}
