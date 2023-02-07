using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealSecurityAssignment.Model;
using RealSecurityAssignment.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RealSecurityAssignment.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly SignInManager<Class> _signInManager;
        private UserManager<Class> _userManager { get; }
        public ChangePasswordModel(SignInManager<Class> signInManager, UserManager<Class>userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [BindProperty]
        [Required]
        public string OldPassword { get; set; }
        [BindProperty]
        [Required]
        [MinLength(12)]
        [ValidateInput]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [ValidateInput]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }
        public async void OnGet()
        {
            
        }
        public async Task<RedirectToPageResult> OnPost()
        {
            var user = await _userManager.GetUserAsync(User);
            PasswordVerificationResult hash = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, OldPassword);
            
            if (hash.ToString().Equals("Success"))
            {
                if (DateTime.Now < user.PasswordAge.Value.AddMinutes(5))
                {
                    System.Diagnostics.Debug.WriteLine("Password Age Failure");
                    TempData["FlashMessage.Type"] = "failure";
                    TempData["FlashMessage.Text"] = string.Format("Password age failure");
                    ModelState.AddModelError(String.Empty, "Password changed recently. Cannot change");
                }
                else
                {
                    user.PasswordAge = DateTime.Now;
                    await _userManager.ChangePasswordAsync(user, OldPassword, Password);
                }
                
                return RedirectToPage("Profile");
            }
            else
            {
                TempData["FlashMessage.Type"] = "failure";
                TempData["FlashMessage.Text"] = string.Format("Current password do not match");
                return RedirectToPage("Profile");
            }
        }
    }
}
