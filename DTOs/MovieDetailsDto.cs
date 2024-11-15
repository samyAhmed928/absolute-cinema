namespace MoviesApi.DTOs
{
	public class MovieDetailsDto
	{
		public int Id { get; set; }
		[MaxLength(250)]
		public string Title { get; set; }
		public int Year { get; set; }
		public double Rate { get; set; }
		[MaxLength(2500)]
		public string StoryLine { get; set; }
		public byte[] Poster { get; set; }

		public int GenreId { get; set; }
		public string GenreName { get; set; }
	}
}
