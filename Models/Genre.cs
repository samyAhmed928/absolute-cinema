namespace MoviesApi.Models
{
	public class Genre
	{
		public int Id { get; set; }
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;
		[MaxLength(250)]
		public string Description { get; set; }= string.Empty;
	}
}
