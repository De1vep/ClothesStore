using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjectPRN221.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPRN221.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly ClothesStoreContext _context;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(ClothesStoreContext context, ILogger<ResetPasswordModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public IActionResult OnGet(string email)
        {
            Email = email;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _context.Users.SingleOrDefault(u => u.Email == Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email not found.");
                return Page();
            }

            user.Password = NewPassword;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password reset successful.");
            return RedirectToPage("/Login");
        }
    }
}
