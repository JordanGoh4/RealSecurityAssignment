using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealSecurityAssignment.Model;
using RealSecurityAssignment.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.DataProtection;

namespace RealSecurityAssignment.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<Class> userManager { get; }
        private SignInManager<Class> signInManager { get; }
        private IWebHostEnvironment _environment;

        private readonly RoleManager<IdentityRole> roleManager;
        [BindProperty]
        //public Register RModel { get; set; }
        public Register RModel { get; set; }
        [BindProperty]
        public IFormFile? Upload { get; set; }
        public RegisterModel(UserManager<Class> userManager,

        SignInManager<Class> signInManager, RoleManager<IdentityRole>roleManager, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            _environment = environment;
        }
        public void OnGet()
        {
        }

        //Save data into the database
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("Photo" + Upload);
                if (Upload != null)
                {
                    System.Diagnostics.Debug.WriteLine("Photo"+Upload);
                    if (Upload.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Upload",
                        "File size cannot exceed 2MB.");
                        return Page();
                    }
                    var uploadsFolder = "Uploads";
                    var imageFile = Guid.NewGuid() + Path.GetExtension(Upload.FileName);
                    var imagePath = Path.Combine(_environment.ContentRootPath,
                    "wwwroot", uploadsFolder, imageFile);
                    using var fileStream = new FileStream(imagePath,
                    FileMode.Create);
                    await Upload.CopyToAsync(fileStream);
                    RModel.Photo = string.Format("/{0}/{1}", uploadsFolder,
                    imageFile);
                }
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                string encrypted = protector.Protect(RModel.Credit);
                var user = new Class()
                {
                    Name = HtmlEncoder.Default.Encode(RModel.Name),
                    Credit = encrypted,
                    Gender = HtmlEncoder.Default.Encode(RModel.Gender),
                    PhoneNumber = HtmlEncoder.Default.Encode(RModel.Mobile.ToString()),
                    Address = HtmlEncoder.Default.Encode(RModel.Address),
                    Email = HtmlEncoder.Default.Encode(RModel.Email),
                    Photo = HtmlEncoder.Default.Encode(RModel.Photo),
                    About = HtmlEncoder.Default.Encode(RModel.About),
                    UserName = HtmlEncoder.Default.Encode(RModel.Email),
                    PasswordAge = DateTime.Now,
                    TwoFactorEnabled = true,
                    EmailConfirmed = true
                };
                //Create the Admin role if NOT exist
                IdentityRole role = await roleManager.FindByIdAsync("Admin");
                if (role == null)
                {
                    System.Diagnostics.Debug.WriteLine("Testing Role");
                    IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError("", "Create role admin failed");
                    }
                }
                

                var result = await userManager.CreateAsync(user, RModel.Password);
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
        

        public string EncryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }

        //public async Task SendVerificationEmailAsync(string emailAddress, string verificationLink)
        //{
        //    var fromAddress = new MailAddress("noreply@example.com", "Example App");
        //    var toAddress = new MailAddress(emailAddress);
        //    const string fromPassword = "password";
        //    const string subject = "Verify Your Email Address";
        //    string body = $"Please click the following link to verify your email address: {verificationLink}";

        //    var smtp = new SmtpClient
        //    {
        //        Host = "appsec1457@gmail.com",
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        //    };
        //    using (var message = new MailMessage(fromAddress, toAddress)
        //    {
        //        Subject = subject,
        //        Body = body
        //    })
        //    {
        //        await smtp.SendMailAsync(message);
        //    }
        //}

    }
}
