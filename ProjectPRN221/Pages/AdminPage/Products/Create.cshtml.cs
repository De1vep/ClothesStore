using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.AdminPage.Products
{
    public class CreateModel : PageModel
    {
        private readonly ClothesStoreContext _context;

        public CreateModel(ClothesStoreContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["page"] = "Admin";
            ViewData["CategoryName"] = new SelectList(_context.Categories.Select(c => c.CategoryName).Distinct().OrderBy(c => c).ToList());
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;
        [BindProperty]
        public string SelectedCategoryName { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["page"] = "Admin";
            var selectedCategory = _context.Categories.FirstOrDefault(c => c.CategoryName == SelectedCategoryName);

            if (selectedCategory != null)
            {
                Product.CategoryId = selectedCategory.CategoryId;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid category selected.");
                ViewData["CategoryName"] = new SelectList(_context.Categories, "CategoryName", "CategoryName");
                return Page();
            }


            var fileName = Path.GetFileName(Product.ProductImageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/product", fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await Product.ProductImageFile.CopyToAsync(fileStream);
            }

            Product.ProductImage = fileName;
            //if (!ModelState.IsValid || _context.Products == null || Product == null)
            //  {
            //      return Page();
            //  }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
