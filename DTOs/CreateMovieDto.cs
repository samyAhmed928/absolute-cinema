namespace MoviesApi.DTOs
{
	public class CreateMovieDto
	{
		[MaxLength(250)]
		public string Title { get; set; }
		public int Year { get; set; }
		[MaxLength(2500)]
		public string StoryLine { get; set; }
		public IFormFile? Poster { get; set; }

		public int GenreId { get; set; }
	}
}
