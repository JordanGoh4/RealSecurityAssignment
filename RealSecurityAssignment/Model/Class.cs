using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RealSecurityAssignment.Model
{
	public class Class : IdentityUser
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Credit { get; set; }
		[Required]
		public string Gender { get; set; }

		[Required]
		public string Address { get; set; }

		[Required]
		public string About { get; set; }

		public string? Photo { get; set; }
		public DateTime? PasswordAge { get; set; }

	}
}
