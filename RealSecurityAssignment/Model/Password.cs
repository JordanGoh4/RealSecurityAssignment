using System.ComponentModel.DataAnnotations;

namespace RealSecurityAssignment.Model
{
    public class Password
    {
        [Required]
        public string Password2 { get; set; }
        [Required]
        public string Confirm { get; set; }
    }
}
