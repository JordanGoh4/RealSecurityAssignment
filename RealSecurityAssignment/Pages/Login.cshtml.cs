using AspNetCore.ReCaptcha;
using Cuemon.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.Security.Application;
using RealSecurityAssignment.Model;
using RealSecurityAssignment.Services;
using RealSecurityAssignment.ViewModels;
using System.Security.Claims;
using System.Text;

namespace RealSecurityAssignment.Pages
{
    [ValidateReCaptcha]
    [ValidateAntiForgeryToken]
    public class LoginModel : PageModel
    {

        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<Class> signInManager;
        private UserManager<Class> userManager { get; }
        private readonly AuditServices _auditServices;
        private EmailSender _emailsender;
        public LoginModel(SignInManager<Class> signInManager, AuditServices auditServices, UserManager<Class> userManager, EmailSender emailSender)
        {
            this.signInManager = signInManager;
            this._auditServices = auditServices;
            this.userManager = userManager;
            _emailsender = emailSender;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                Class? user = await userManager.FindByNameAsync(LModel.Email);
                if(user != null && await userManager.CheckPasswordAsync(user, LModel.Password))
                {
                    await userManager.UpdateSecurityStampAsync(user);
                }
                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
                LModel.RememberMe, true);
                System.Diagnostics.Debug.WriteLine(LModel.Email + "Email");
                Audit audit = new Audit();
                
                if (identityResult.RequiresTwoFactor)
                {
                    var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    await _emailsender.Executeotp("OTP", token, user.Email);
                    return RedirectToPage("OTP", new {email = user.Email});
                }else if(identityResult.Succeeded)
                {
                    audit.Action = "Login";
                    audit.CreatedDate = DateTime.Now;
                    audit.Username = user.Id.ToString();
                    _auditServices.Add(audit);
                    return RedirectToPage("Profile");
                }
                else if (identityResult.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Your account is locked out wait for 3mins");
                    //if (user != null)
                    //{
                    //    if (DateTime.Now > user.LockoutEnd.Value.AddMinutes(3))
                    //    {
                    //        var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    //        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    //        var confirmation = Url.Page("/ResetPassword", pageHandler: null, values: new { email = user.Email, token }, protocol: Request.Scheme);
                    //        await _emailsender.Execute("Account Recovery", confirmation, user.Email);
                    //        TempData["FlashMessage.Type"] = "success";
                    //        TempData["FlashMessage.Text"] = string.Format("Email has been sent for verification");
                    //        return Redirect("/Login");
                    //    }
                    //    else
                    //    {
                    //        System.Diagnostics.Debug.WriteLine("Lockout havent end");
                    //        TempData["FlashMessage.Type"] = "failure";
                    //        TempData["FlashMessage.Text"] = string.Format("Lockout haven't end");
                    //    }

                    //}
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(user.Email + "Email");
                    audit.Action = "Failed Login";
                    audit.Username = user.Id;
                    audit.CreatedDate = DateTime.Now;
                    _auditServices.Add(audit);
                    int fails = await signInManager.UserManager.GetAccessFailedCountAsync(user);
                    int total = signInManager.Options.Lockout.MaxFailedAccessAttempts;
                    string message = $"Login failed. Unsuccessful attempts {fails} of {total}";
                    ModelState.AddModelError(string.Empty, message);
                }
                ModelState.AddModelError("", "Username or Password incorrect");
            }
            return Page();
        }

    }
}
