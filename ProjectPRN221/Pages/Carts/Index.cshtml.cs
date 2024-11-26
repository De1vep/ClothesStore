using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.Carts
{
    public class IndexModel : PageModel
    {
        private readonly ProjectPRN221.Models.ClothesStoreContext _context;

        public IndexModel(ProjectPRN221.Models.ClothesStoreContext context)
        {
            _context = context;
        }

        public IList<Cart> Cart { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Carts != null)
            {
                Cart = await _context.Carts
                .Include(c => c.Product)
                .Include(c => c.User).ToListAsync();
            }
        }
    }
}
