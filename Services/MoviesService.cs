using MoviesApi.Models;

namespace MoviesApi.Services
{
	public class MoviesService(ApplictionDbContext _context) : IMoviesService
	{

		public async Task<IEnumerable<Movie>> GetAll(int genreId = 0)
		{
			var movies = await _context.Movies
				        .Where(m=>m.GenreId == genreId||genreId==0)
						.OrderByDescending(m => m.Rate)
						.Include(m => m.Genre)
						.ToListAsync();
			return movies;
		}

		public async Task<Movie> GetById(int id)
		{
			return await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);
		}

		public async Task<Movie> Add(Movie movie)
		{
			await _context.AddAsync(movie);
			_context.SaveChanges();
			return movie;
		}

		public Movie Update(Movie movie)
		{
			throw new NotImplementedException();
		}

		public Movie Delete(Movie movie)
		{
			throw new NotImplementedException();
		}

	}
}
