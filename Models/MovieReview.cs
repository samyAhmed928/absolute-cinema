namespace MoviesApi.Models
{
	public class MovieReview
	{
		public int Id { get; set; }
		public int MovieId { get; set; }
		public Movie Movie { get; set; }

		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		[MaxLength(2500)]
		public string Comment { get; set; } =string.Empty;
		public DateTime CreatedAt { get; set; }=DateTime.Now;
	}
}
