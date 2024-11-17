using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Models;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MoviesController(IMoviesService _moviesService,IGenreService _genreService,IMapper _mapper,UserManager<ApplicationUser> _usermanger,ILogger<MoviesController>_logger) : ControllerBase
	{

		private readonly List<string>_allowedExtensions=new List<string> { ".jpg",".png"};
		private long _maxAllowedPosterSize = 1 * 1024 * 1024;

		[HttpGet]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetAllAsync()
		{
			var movies = await _moviesService.GetAll();
			var data=_mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
			return Ok(data);
		}
		[HttpGet("{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetByIdAsync(int id)
		{
			var movie =await _moviesService.GetById(id);
			if (movie == null)
				return NotFound("No movie with this id");

			var data = _mapper.Map<MovieDetailsDto>(movie);

			return Ok(data);
		}
		[HttpGet("GetByGenreId")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetMoviesByGenreIDAsync(int genreId)
		{
			var movies = await _moviesService.GetAll(genreId);
			if (movies.Count()==0)
				return NotFound("No movies within this genre");

			var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies); 
			return Ok(data);
		}
		[HttpPost]
		[Authorize(Roles ="Admin")]
		public async Task<IActionResult> CreateAsync([FromForm]CreateMovieDto dto)
		{

			var user = await _usermanger.GetUserAsync(User);
			if (user is null)
				return Unauthorized("User not logged in.");
			

			if (dto.Poster is null)
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

			try
			{
				using var dataStream = new MemoryStream();
				await dto.Poster.CopyToAsync(dataStream);

				var movie = _mapper.Map<Movie>(dto);
				movie.Poster = dataStream.ToArray();
				movie.AdminId = user.Id;

				await _moviesService.Add(movie);

				// Return Created status code with the movie data
				return Ok(movie);
			}
			catch (Exception ex)
			{
				// Log the exception (you can log to a file, database, etc.)
				_logger.LogError(ex, "An error occurred while creating the movie.");
				return StatusCode(500, "An unexpected error occurred.");
			}
		}
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]

		public async Task<IActionResult> UpdateAsync(int id, [FromForm] CreateMovieDto dto)
		{
			var user = await _usermanger.GetUserAsync(User);
			if (user is null)
				return Unauthorized("User not logged in.");

			var movie = await _moviesService.GetById(id);

			if (movie == null)
				return NotFound($"No movie was found with ID :{id}");

			var isValidGenre = await _genreService.IsValidGenre(dto.GenreId);
			if (!isValidGenre)
				return BadRequest("Invalid Genre Id");
			if (movie.AdminId!=user.Id)
			{
				return Unauthorized("movie doesn't belong to this admin");
			}
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


			_moviesService.Update(movie);

			return Ok(movie);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			var user = await _usermanger.GetUserAsync(User);
			if (user is null)
				return Unauthorized("User not logged in.");

			var movie = await _moviesService.GetById(id);

			if (movie == null)
				return NotFound($"No movie was found with ID :{id}");
			if (movie.AdminId != user.Id)
			{
				return Unauthorized("movie doesn't belong to this admin");
			}
			_moviesService.Delete(movie);

			return Ok(movie);
		}

		[HttpPost("Rate/{movieid}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> RateMovieAsync(int movieid, int rating)
		{
			var user = await _usermanger.GetUserAsync(User);
			if (user is null)
				return Unauthorized("User not logged in.");
			var movie = await _moviesService.GetById(movieid);

			if (movie == null)
				return NotFound($"No movie was found with ID :{movieid}");
			if (rating < 0 || rating > 5)
				return BadRequest("Rating Value must be between 1 and 5");
			// Check if the user has already rated this movie
			var existingRating = movie.movieRatings.FirstOrDefault(r => r.UserId == user.Id);
			if (existingRating != null)
			{
				// Update the existing rating
				existingRating.Rating = rating;
			}
			else
			{
				// Add a new rating
				var movieRating = new MovieRating
				{
					MovieId = movie.Id,
					UserId = user.Id,
					Rating = rating
				};

				movie.movieRatings.Add(movieRating);
			}
			_moviesService.Update(movie);
			return Ok(movie);
		}

		[HttpPost("Review/{movieid}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> ReviewMovieAsync(int movieid, string Comment)
		{
			var user = await _usermanger.GetUserAsync(User);
			if (user is null)
				return Unauthorized("User not logged in.");
			var movie = await _moviesService.GetById(movieid);

			if (movie == null)
				return NotFound($"No movie was found with ID :{movieid}");

			// Check if the user has already rated this movie

			
				// Add a new rating
				var movieReview = new MovieReview 
				{
					MovieId = movie.Id,
					UserId = user.Id,
					Comment = Comment
				};

			movie.MovieReviews.Add(movieReview);
			
			_moviesService.Update(movie);
			return Ok(movie);
		}
	}
}
