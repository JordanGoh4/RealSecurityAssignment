using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RealSecurityAssignment.ViewModels
{
    public class Login
    {
        [Required]
        [ValidateInput]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [ValidateInput]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class ValidateInputAttribute : ValidationAttribute
    {
        private readonly string pattern = @"^[a-zA-Z0-9\s,\.\?\!\@]+$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            System.Diagnostics.Debug.WriteLine("value" + value.ToString());
            if (!Regex.IsMatch(value.ToString(), pattern))
            {
                return new ValidationResult("Invalid input. Only alphanumeric characters, spaces, and certain punctuation marks (`,`, `.`, `?`,`@` and `!`) are allowed.");
            }

            return ValidationResult.Success;
        }
    }
}
