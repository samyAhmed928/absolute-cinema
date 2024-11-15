using Microsoft.AspNetCore.Identity;

namespace MoviesApi.Models
{
	public class ApplicationUser:IdentityUser
	{
		[MaxLength(50)]
		public string FirstName { get; set; }
		[MaxLength(50)]
		public string LastName { get; set; }
	}
}
