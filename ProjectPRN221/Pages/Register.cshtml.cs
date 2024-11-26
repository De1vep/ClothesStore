using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjectPRN221.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProjectPRN221.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ClothesStoreContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(ClothesStoreContext context, ILogger<RegisterModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        [Required]
        public string Fullname { get; set; }

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
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                foreach (var error in errors)
                {
                    _logger.LogError(error.ErrorMessage);
                }
                return Page();
            }

            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.SingleOrDefault(u => u.Email == Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email is already in use.");
                    return Page();
                }

                var verificationLink = Url.Page("/VerifyEmail", null, new { fullname = Fullname, email = Email, password = Password }, Request.Scheme);
                SendVerificationEmail(Email, verificationLink);

                _logger.LogInformation("Verification email sent.");
                return RedirectToPage("/EmailConfirmation");
            }

            return Page();
        }

        private void SendVerificationEmail(string email, string verificationLink)
        {
            var fromAddress = new MailAddress("anhpvhe176182@fpt.edu.vn", "Grocery Mart");
            var toAddress = new MailAddress(email);
            const string fromPassword = "kvqafagehuvtivyy";
            const string subject = "Email Verification";
            string body = $"Please verify your email by clicking <a href='{verificationLink}'>here</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}
