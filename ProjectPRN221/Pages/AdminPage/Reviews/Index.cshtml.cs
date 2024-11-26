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
    public class IndexModel : PageModel
    {
        private readonly ProjectPRN221.Models.ClothesStoreContext _context;

        public IndexModel(ProjectPRN221.Models.ClothesStoreContext context)
        {
            _context = context;
        }

        public IList<Review> Review { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ViewData["page"] = "Admin";
            if (_context.Reviews != null)
            {
                Review = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User).ToListAsync();
            }
        }
    }
}
