using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.AdminPage.Reviews
{
    public class DetailsModel : PageModel
    {
        private readonly ProjectPRN221.Models.ClothesStoreContext _context;

        public DetailsModel(ProjectPRN221.Models.ClothesStoreContext context)
        {
            _context = context;
        }

      public Review Review { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["page"] = "Admin";
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }
            else 
            {
                Review = review;
            }
            return Page();
        }
    }
}
