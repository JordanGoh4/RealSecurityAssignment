using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealSecurityAssignment.Model;
using RealSecurityAssignment.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace RealSecurityAssignment.Pages
{
    [BindProperties]
    public class GoogleModel : PageModel
    {
        private readonly UserManager<Class> userManager;
        private readonly SignInManager<Class> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public GoogleModel(UserManager<Class>_userManager, SignInManager<Class> _signInManager, RoleManager<IdentityRole> _roleManager)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
        }
        
        [Required]
        [ValidateInput]
        public string Name { get; set; }
        [Required]
        [ValidateInput]
        [CreditCard]
        public string Credit { get; set; }
        [Required]
        [ValidateInput]
        public string Gender { get; set; }
        [Required]
        [ValidateInput]
        public int Mobile { get; set; }
        [Required]
        [ValidateInput]
        public string Address { get; set; }
        [Required]
        [ValidateInput]
        public string About { get; set; }
        [Required]
        [ValidateInput]
        public string Email { get; set; }
        public void OnGet(string email, string name)
        {
            Email = email;
            Name = name;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                string encrypted = protector.Protect(Credit);
                var user = new Class()
                {
                    Name = HtmlEncoder.Default.Encode(Name),
                    Credit = encrypted,
                    Gender = HtmlEncoder.Default.Encode(Gender),
                    PhoneNumber = HtmlEncoder.Default.Encode(Mobile.ToString()),
                    Address = HtmlEncoder.Default.Encode(Address),
                    Email = HtmlEncoder.Default.Encode(Email),
                    Photo = HtmlEncoder.Default.Encode("Photo"),
                    About = HtmlEncoder.Default.Encode(About),
                    UserName = HtmlEncoder.Default.Encode(Email)
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //Add users to Admin Role
                    var result2 = await userManager.AddToRoleAsync(user, "Admin");
                    await signInManager.SignInAsync(user, false);
                    System.Diagnostics.Debug.WriteLine(result2.Succeeded);
                    TempData["FlashMessage.Type"] = "success";
                    TempData["FlashMessage.Text"] = string.Format("Account has been registered");
                    return RedirectToPage("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }
    }
}
