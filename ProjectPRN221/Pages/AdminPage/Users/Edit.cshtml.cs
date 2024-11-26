using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.AdminPage.Users
{
    public class EditModel : PageModel
    {
        private readonly ClothesStoreContext _context;

        public EditModel(ClothesStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int UserId { get; set; }

        [BindProperty]
        public string Fullname { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public string Role { get; set; }

        [BindProperty]
        public bool NotifyUserByEmail { get; set; }

        public IActionResult OnGet(int id)
        {
            ViewData["page"] = "Admin";
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            UserId = user.UserId;
            Fullname = user.Fullname;
            Email = user.Email;
            Role = user.Role;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["page"] = "Admin";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }

            var userInDb = _context.Users.FirstOrDefault(u => u.UserId == UserId);
            if (userInDb == null)
            {
                return NotFound();
            }

            userInDb.Fullname = Fullname;
            userInDb.Email = Email;
            userInDb.Role = Role;

            if (!string.IsNullOrEmpty(Password))
            {
                userInDb.Password = Password;
            }

            await _context.SaveChangesAsync();

            if (NotifyUserByEmail)
            {
                // Logic to send notification email to the user
            }

            return RedirectToPage("CustomerManagement");
        }
    }
}
