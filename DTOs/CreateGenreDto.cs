namespace MoviesApi.DTOs
{
	public class CreateGenreDto
	{
		[MaxLength(100)]
		public string Name { get; set; }
		public string Description { get; set; } = string.Empty;

	}
}
