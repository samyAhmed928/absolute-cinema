using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Models;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MoviesController(IMoviesService _moviesService,IGenreService _genreService,IMapper _mapper) : ControllerBase
	{

		private readonly List<string>_allowedExtensions=new List<string> { ".jpg",".png"};
		private long _maxAllowedPosterSize = 1 * 1024 * 1024;

		[HttpGet]
		public async Task<IActionResult> GetAllAsync()
		{
			var movies = await _moviesService.GetAll();
			var data=_mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
			return Ok(data);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetByIdAsync(int id)
		{
			var movie =await _moviesService.GetById(id);
			if (movie == null)
				return NotFound("No movie with this id");

			var data = _mapper.Map<MovieDetailsDto>(movie);

			return Ok(data);
		}
		[HttpGet("GetByGenreId")]
		public async Task<IActionResult> GetMoviesByGenreIDAsync(int genreId)
		{
			var movies = await _moviesService.GetAll(genreId);
			//TODO: map movies to DTO
			return Ok(movies);
		}
		[HttpPost]
		[Authorize(Roles ="Admin")]
		public async Task<IActionResult> CreateAsync([FromForm]CreateMovieDto dto)
		{
			if (dto.Poster==null)
			{
				return BadRequest("Poster is Requried");
			}
			if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
			{
				return BadRequest("Only .png and .jpg are allowed");
			}
			if (dto.Poster.Length>_maxAllowedPosterSize)
			{
				return BadRequest("Max Size Allowed is 1 MB");
			}

			var isValidGenre = await _genreService.IsValidGenre(dto.GenreId);
			if (!isValidGenre)
				return BadRequest("Invalid Genre Id");

			using var dataStream= new MemoryStream();

			await dto.Poster.CopyToAsync(dataStream);
			var movie = _mapper.Map<Movie>(dto);
			movie.Poster=dataStream.ToArray();
			await _moviesService.Add(movie);

			return Ok(movie);
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync(int id, [FromForm] CreateMovieDto dto)
		{
			var movie = await _moviesService.GetById(id);

			if (movie == null)
				return NotFound($"No movie was found with ID :{id}");

			var isValidGenre = await _genreService.IsValidGenre(dto.GenreId);
			if (!isValidGenre)
				return BadRequest("Invalid Genre Id");

			if (dto.Poster !=null)
			{
				if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
				{
					return BadRequest("Only .png and .jpg are allowed");
				}
				if (dto.Poster.Length > _maxAllowedPosterSize)
				{
					return BadRequest("Max Size Allowed is 1 MB");
				}

				using var dataStream = new MemoryStream();

				await dto.Poster.CopyToAsync(dataStream);
				movie.Poster = dataStream.ToArray();
			}
			movie.Title = dto.Title;
			movie.Year = dto.Year;
			movie.GenreId = dto.GenreId;
			movie.StoryLine = dto.StoryLine;
			movie.Rate = dto.Rate;


			_moviesService.Update(movie);

			return Ok(movie);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			var movie = await _moviesService.GetById(id);

			if (movie == null)
				return NotFound($"No movie was found with ID :{id}");

			_moviesService.Delete(movie);

			return Ok(movie);
		}
	}
}
