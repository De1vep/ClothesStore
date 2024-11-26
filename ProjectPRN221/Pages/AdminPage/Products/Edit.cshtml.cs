using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;
using System.IO;
using System.Threading.Tasks;

namespace ProjectPRN221.Pages.AdminPage.Products
{
    public class EditModel : PageModel
    {
        private readonly ClothesStoreContext _context;

        public EditModel(ClothesStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        [BindProperty]
        public IFormFile? NewImage { get; set; }

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
            Product = product;

            var categoryNames = await _context.Categories
                                        .Select(c => c.CategoryName)
                                        .Distinct()
                                        .ToListAsync();
            ViewData["CategoryName"] = new SelectList(categoryNames);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["page"] = "Admin";

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var productToUpdate = await _context.Products
                                                .Include(p => p.Category)
                                                .FirstOrDefaultAsync(p => p.ProductId == Product.ProductId);

            if (productToUpdate == null)
            {
                return NotFound();
            }

            if (NewImage != null && NewImage.Length > 0)
            {
                var filePath = Path.Combine("wwwroot/assets/img/product", Path.GetFileName(NewImage.FileName));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await NewImage.CopyToAsync(stream);
                }

                productToUpdate.ProductImage = Path.GetFileName(NewImage.FileName);
            }

            productToUpdate.ProductName = Product.ProductName;
            productToUpdate.ProductDescription = Product.ProductDescription;
            productToUpdate.Price = Product.Price;
            productToUpdate.Quantity = Product.Quantity;
            productToUpdate.Category.CategoryName = Product.Category.CategoryName;

            _context.Attach(productToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
