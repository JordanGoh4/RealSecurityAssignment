using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealSecurityAssignment.Model;
using RealSecurityAssignment.Services;

namespace RealSecurityAssignment.Pages
{
    public class OTPModel : PageModel
    {
        private UserManager<Class> userManager { get; }
        private SignInManager<Class> signInManager { get; }
        private readonly AuditServices _auditServices;
        [BindProperty]
        public string OTP { get; set; }
        public OTPModel(UserManager<Class>_userManager, SignInManager<Class>_signInManager, AuditServices auditServices)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            _auditServices = auditServices;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            var confirm = await signInManager.TwoFactorSignInAsync("Email", OTP, false, false);
            Audit audit = new Audit();
            System.Diagnostics.Debug.WriteLine(confirm);
           
            if (confirm.Succeeded)
            {
                System.Diagnostics.Debug.WriteLine("OTP");
                await signInManager.SignInAsync(user,false);
                audit.Action = "Login";
                audit.CreatedDate = DateTime.Now;
                audit.Username = user.Id.ToString();
                _auditServices.Add(audit);
                return RedirectToPage("Profile");
            }
            else
            {
                return RedirectToPage("Login");
            }
        }
    }
}
