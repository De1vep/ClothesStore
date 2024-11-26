using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly ClothesStoreContext _context;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(ClothesStoreContext context, ILogger<ForgotPasswordModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public void OnGet()
        {
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

            var resetLink = Url.Page("/ResetPassword", null, new { email = Email }, Request.Scheme);

            SendResetEmail(Email, resetLink);

            _logger.LogInformation("Password reset link sent.");
            return RedirectToPage("/Login");
        }

        private void SendResetEmail(string email, string resetLink)
        {
            var fromAddress = new MailAddress("anhpvhe176182@fpt.edu.vn", "Grocery Mart");
            var toAddress = new MailAddress(email);
            const string fromPassword = "kvqafagehuvtivyy";
            const string subject = "Password Reset";
            string body = $"Please reset your password by clicking <a href='{resetLink}'>here</a>";

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
