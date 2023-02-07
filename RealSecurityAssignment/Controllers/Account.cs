using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCompany.Services;
using RealSecurityAssignment.Model;
using RealSecurityAssignment.Services;
using System.Security.Claims;

namespace MyCompany.Controllers
{
	public class Account : Controller
	{
		private UserManager<Class> _userManager { get; }

        private readonly UserServices _userService;
        private IWebHostEnvironment _environment;
        private EmailSender _emailsender;
        private SignInManager<Class> signInManager { get; }

        public Account(UserServices userService, IWebHostEnvironment environment, UserManager<Class> userManager, SignInManager<Class> signInManager, EmailSender emailsender)
        {
            _userService = userService;
            _environment = environment;
            _userManager = userManager;
            this.signInManager = signInManager;
            _emailsender = emailsender;
        }
        
        public IActionResult GoogleLogin(string returnUrl = null)
        {
            //await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            //{
            //    RedirectUri = Url.Action("GoogleCallback", "Account", new { returnUrl })
            //});
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Account", new { returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }
        public async Task<IActionResult> GoogleCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return Redirect("/Login");
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Redirect("/Register");
            }

            // Obtain the user information
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            var pfp = info.Principal.FindFirstValue("image");
            

            // Use the user information for your application logic

            // Redirect to the original URL
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Redirect(returnUrl ?? "/Google?email=" + email + "&name=" + name + "&pfp=" + pfp);
            }
            else
            {
                await signInManager.SignInAsync(user, true);
				TempData["FlashMessage.Type"] = "success";
				TempData["FlashMessage.Text"] = string.Format("Successful Login");
				return Redirect("/");
            }
        }
    }
}
