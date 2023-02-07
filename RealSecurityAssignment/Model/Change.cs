using System.ComponentModel.DataAnnotations;

namespace RealSecurityAssignment.Model
{
    public class Change
    {
        [Required]
        public string Email { get; set; }

    }
}
