using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using RealSecurityAssignment.Model;
using RealSecurityAssignment.Services;
using System.Text;

namespace RealSecurityAssignment.Pages
{
    public class ForgotModel : PageModel
    {
        private readonly SignInManager<Class> _signInManager;
        private UserManager<Class> _userManager { get; }
        private readonly AuditServices _auditServices;
        private EmailSender _emailsender;
        [BindProperty]
        public Class User { get; set; } = new Class();
        public ForgotModel(UserManager<Class> userManager, SignInManager<Class> signInManager, AuditServices auditServices, EmailSender emailsender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _auditServices = auditServices;
            _emailsender = emailsender;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.FindByEmailAsync(User.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var confirmation = Url.Page("/ResetPassword", pageHandler: null, values: new { email = user.Email, token }, protocol:Request.Scheme);
                await _emailsender.Execute("Account Verfication", confirmation, user.Email);
                TempData["FlashMessage.Type"] = "success";
                TempData["FlashMessage.Text"] = string.Format("Email has been sent for verification");
                return Redirect("/");
            }
            else
            {
                return RedirectToPage("Error");
            }
            return RedirectToPage("Login");
        }
    }
}
