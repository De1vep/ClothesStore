using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.AdminPage.Users
{
    public class IndexModel : PageModel
    {
        private readonly ClothesStoreContext _context;

        public IndexModel(ClothesStoreContext context)
        {
            _context = context;
        }

        public List<User> Users { get; set; }

        public void OnGet()
        {
            ViewData["page"] = "Admin";
            Users = _context.Users.ToList();
        }
    }
}
