using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.AdminPage.Reviews
{
    public class CreateModel : PageModel
    {
        private readonly ProjectPRN221.Models.ClothesStoreContext _context;

        public CreateModel(ProjectPRN221.Models.ClothesStoreContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["page"] = "Admin";
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return Page();
        }

        [BindProperty]
        public Review Review { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["page"] = "Admin";
            if (!ModelState.IsValid || _context.Reviews == null || Review == null)
            {
                return Page();
            }

            _context.Reviews.Add(Review);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
