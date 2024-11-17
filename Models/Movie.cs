namespace MoviesApi.Models
{
	public class Movie
	{
		public int Id { get; set; }
		[MaxLength(250)]
		public string Title { get; set; }
		public int Year { get; set; }
		public double Rate { get; set; }
		[MaxLength(2500)]
		public string StoryLine { get; set; }
		public byte[] Poster { get; set; }

		public int GenreId {  get; set; }
		public Genre Genre { get; set; }

		public string AdminId { get; set; }
		public ApplicationUser Admin { get; set; }

		public IList<MovieRating> movieRatings { get; set; }=new List<MovieRating>();
		public IList<MovieReview> MovieReviews { get; set; }=new List<MovieReview>();


	}
}
