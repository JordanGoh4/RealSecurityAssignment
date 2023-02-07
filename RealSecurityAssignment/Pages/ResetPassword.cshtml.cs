using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Help.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using RealSecurityAssignment.Model;
using RealSecurityAssignment.Services;

namespace RealSecurityAssignment.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<Class> _userManager;
        private readonly UserServices _userServices;

        public ResetPasswordModel(UserManager<Class> userManager, UserServices userServices)
        {
            _userManager = userManager;
            _userServices = userServices;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 12)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string Code { get; set; }
            
        }
        public IActionResult OnGet(string email = null, string token = null)
        {
            
            if (token == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token)),
                    Email = email,
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Class user = await _userManager.FindByEmailAsync(Input.Email);
            System.Diagnostics.Debug.WriteLine(Input.Password);
            System.Diagnostics.Debug.WriteLine(Input.Code);
            //var token2 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var compare = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, Input.Password).ToString().Equals("Success");
            if (compare)
            {
                if(user.PasswordAge == null)
                {
                    user.PasswordAge = DateTime.Now;
                }else
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
                    await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
                }
                
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Password Same");
                TempData["FlashMessage.Type"] = "failure";
                TempData["FlashMessage.Text"] = string.Format("Password is the same as before");
            }
            
            return Page();
        }
    }
}
