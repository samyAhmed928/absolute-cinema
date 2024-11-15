using MoviesApi.Models;

namespace MoviesApi.Services
{
	public interface IGenreService
	{
		Task<IEnumerable<Genre>> GetAll();
		Task<Genre> GetGenreByID(int id);
		Task<Genre> Add(Genre genre);
		Genre Update(Genre genre);
		Genre Delete(Genre genre);
		Task<bool>IsValidGenre(int id);
	}
}
