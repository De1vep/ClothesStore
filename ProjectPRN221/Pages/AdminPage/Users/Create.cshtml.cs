using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.AdminPage.Users
{
    public class CreateModel : PageModel
    {
        private readonly ClothesStoreContext _context;

        public CreateModel(ClothesStoreContext context)
        {
            _context = context;
        }

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

        public void OnGet() // This method must have a return type
        {
            // Any initialization code
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

            var newUser = new User
            {
                Fullname = Fullname,
                Email = Email,
                Password = Password,
                Role = Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            if (NotifyUserByEmail)
            {
                // Logic to send notification email to the user
            }

            return RedirectToPage("/AdminPage/Users/Index"); // Assuming you have an Index page for listing users
        }
    }
}
