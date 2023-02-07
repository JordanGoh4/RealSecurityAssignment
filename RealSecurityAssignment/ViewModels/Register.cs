using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
//using System.Web.Mvc;

namespace RealSecurityAssignment.ViewModels
{
    public class Register
    {
     
        [Required]
        [ValidateInput]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "The Email field is not a valid email address.")]
        public string Email { get; set; }
        [Required]
        [MinLength(12)]
        [ValidateInput]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [ValidateInput]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }
        [Required]
        [ValidateInput]
        public string Name { get; set; }
        [Required]
        [CreditCard (ErrorMessage ="Invalid Credit Card")]
        [ValidateInput]
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
        public string? Photo { get; set; }
    }
    
}
