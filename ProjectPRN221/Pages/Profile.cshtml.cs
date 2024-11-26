using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectPRN221.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProjectPRN221.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly ClothesStoreContext _context;

        public ProfileModel(ClothesStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Fullname { get; set; }

        [BindProperty]
        public string Email { get; set; }


        [BindProperty]
        public string Password { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userId);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            Fullname = user.Fullname;
            Email = user.Email;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userId);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            if (ModelState.IsValid)
            {
                user.Fullname = Fullname;
                user.Email = Email;
                user.Password = Password;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                // Update session
                HttpContext.Session.SetString("UserName", user.Fullname);

                return RedirectToPage("/Profile");
            }

            return Page();
        }
    }
}
