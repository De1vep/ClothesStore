using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectPRN221.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPRN221.Pages.AdminPage.Users
{
    public class DeleteModel : PageModel
    {
        private readonly ClothesStoreContext _context;

        public DeleteModel(ClothesStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["page"] = "Admin";
            var user = await _context.Users.FindAsync(User.UserId);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToPage("CustomerManagement");
        }
    }
}
