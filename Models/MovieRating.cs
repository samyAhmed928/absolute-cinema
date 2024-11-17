using Microsoft.AspNetCore.Identity;

namespace MoviesApi.Models
{
	public class MovieRating
	{
		public int MovieId { get; set; }
		public Movie Movie { get; set; }

		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		public int Rating { get; set; } // 1 to 5 stars
		
	}
}
