using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Models;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController(IAuthService _authservice) : ControllerBase
	{
		[HttpPost("Register")]
		public async Task<IActionResult>RegisterAsync([FromBody]RegisterModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result=await _authservice.RegisterAsync(model);
			if (!result.IsAuthenticated)
			{
				return BadRequest(result.Message);
			}

			return Ok(new
			{
				token=result.Token,
				exprieson=result.ExpireOn
			});
		}

		[HttpPost("Token")]
		public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result = await _authservice.GetTokenAsync(model);
			if (!result.IsAuthenticated)
			{
				return BadRequest(result.Message);
			}

			return Ok(new
			{
				token = result.Token,
				exprieson = result.ExpireOn
			});
		}

		[HttpPost("AddRole")]
		public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result = await _authservice.AddRoleAsync(model);
			if (!string.IsNullOrEmpty(result))
			{
				return BadRequest(result);
			}

			return Ok(model);
	
			
		}
	}
}
