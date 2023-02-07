using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealSecurityAssignment.Model;
using RealSecurityAssignment.Services;
using RealSecurityAssignment.ViewModels;

namespace RealSecurityAssignment.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<Class> signInManager;
        private readonly AuditServices _auditServices;
        private UserManager<Class> _userManager { get; }
        public LogoutModel(SignInManager<Class> signInManager, UserManager<Class> userManager, AuditServices auditServices)
        {
            this.signInManager = signInManager;
            this._auditServices = auditServices;
            this._userManager = userManager;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            Class? user = await _userManager.GetUserAsync(User);
            System.Diagnostics.Debug.WriteLine(user.UserName + "logout");
            Audit audit = new Audit();
            audit.Username = user.Id.ToString();
            audit.Action = "Logout";
            audit.CreatedDate = DateTime.Now;
            _auditServices.Add(audit);
            await signInManager.SignOutAsync();
            return RedirectToPage("Login");
        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("Index");
        }
    }
}
