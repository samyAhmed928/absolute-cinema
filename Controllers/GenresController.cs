using MoviesApi.Models;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GenresController(IGenreService _genreService) : ControllerBase
	{

        [HttpGet]
		public async Task<IActionResult> GetAllAsync()
		{
			var Genres=await _genreService.GetAll();
			return Ok(Genres);
		}
		[HttpPost]
		public async Task<IActionResult> CreateAsync(CreateGenreDto dto)
		{
			var genre = new Genre
			{
				Name= dto.Name,
				Description=dto.Description
			};
			await _genreService.Add(genre);
			return Ok(genre);
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync(int id,[FromBody]CreateGenreDto Dto)
		{
			var genre=await _genreService.GetGenreByID(id);

			if (genre == null)
				return NotFound($"No genre was found with ID :{id}");

			_genreService.Update(genre);

			return Ok(genre);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult>DeleteAsync(int id)
		{
			var genre = await _genreService.GetGenreByID(id);

			if (genre == null)
				return NotFound($"No genre was found with ID :{id}");

			_genreService.Delete(genre);

			return Ok(genre);
		}
	}
}
