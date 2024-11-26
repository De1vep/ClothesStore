using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectPRN221.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPRN221.Pages
{
    public class VerifyEmailModel : PageModel
    {
        private readonly ClothesStoreContext _context;
        private readonly ILogger<VerifyEmailModel> _logger;

        public VerifyEmailModel(ClothesStoreContext context, ILogger<VerifyEmailModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string fullname, string email, string password)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.Email == email);
            if (existingUser == null)
            {
                var newUser = new User
                {
                    Fullname = fullname,
                    Email = email,
                    Password = password,
                    Role = "customer"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("New user registered via email verification.");
            }

            return Page();
        }
    }
}
