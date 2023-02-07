using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealSecurityAssignment.Model;

namespace RealSecurityAssignment.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private UserManager<Class> _userManager { get; }
        public Class _User { get; set; }
        public ProfileModel(UserManager<Class> userManager)
        {
            this._userManager = userManager;
        }
            public async Task OnGetAsync()
        {
            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");
            Class? user = await _userManager.GetUserAsync(User);
            _User = user;
            _User.Credit = protector.Unprotect(_User.Credit);
        }
        public string DecryptString(string encrString)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(encrString);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
            }
            catch (FormatException fe)
            {
                decrypted = "";
            }
            return decrypted;
        }
    }
}
