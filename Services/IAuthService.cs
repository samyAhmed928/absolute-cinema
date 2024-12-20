﻿using MoviesApi.Models;

namespace MoviesApi.Services
{
	public interface IAuthService
	{
		Task<AuthModel> RegisterAsync(RegisterModel model);
		Task<AuthModel> GetTokenAsync(TokenRequestModel model);
		Task<AuthModel> AddRoleAsync(AddRoleModel model);
	}
}
