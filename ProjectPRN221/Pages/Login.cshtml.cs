using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjectPRN221.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProjectPRN221.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ClothesStoreContext _context;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(ClothesStoreContext context, ILogger<LoginModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Kiểm tra các lỗi xác thực
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                foreach (var error in errors)
                {
                    _logger.LogError(error.ErrorMessage);
                }
                return Page();
            }

            if (ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(u => u.Email == Email);
                if (user != null && user.Password == Password)
                {
                    // Lưu UserID và Role vào Session
                    HttpContext.Session.SetString("UserID", user.UserId.ToString());
                    HttpContext.Session.SetString("Role", user.Role);
                    _logger.LogInformation("User logged in.");
                    if (user.Role.Equals("admin"))
                    {
                        return LocalRedirect("/AdminPage");
                    }
                    else { 
                    return LocalRedirect("/Index");
                    
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            return Page();
        }
    }
}
