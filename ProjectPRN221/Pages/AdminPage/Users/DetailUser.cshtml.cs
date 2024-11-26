using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectPRN221.Models;
using System.Linq;

namespace ProjectPRN221.Pages.AdminPage.Users
{
    public class DetailModel : PageModel
    {
        private readonly ClothesStoreContext _context;

        public DetailModel(ClothesStoreContext context)
        {
            _context = context;
        }

        public User User { get; set; }

        public IActionResult OnGet(int id)
        {
            ViewData["page"] = "Admin";
            User = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (User == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
