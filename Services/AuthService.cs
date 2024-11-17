using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoviesApi.Helpers;
using MoviesApi.Models;
using NuGet.Common;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoviesApi.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _usermanger;
		private readonly RoleManager<IdentityRole> _roleManger;
		private readonly JWT _jwt;
		public AuthService(IOptions<JWT> jwt, UserManager<ApplicationUser> usermanger, RoleManager<IdentityRole> roleManger)
		{
			_jwt = jwt.Value;
			_usermanger = usermanger;
			_roleManger = roleManger;
		}



		public async Task<AuthModel> RegisterAsync(RegisterModel model)
		{
			if (await _usermanger.FindByEmailAsync(model.Email) is not null)
				return new AuthModel { Message = "Email is already registerd" };

			if (await _usermanger.FindByNameAsync(model.UserName) is not null)
				return new AuthModel { Message = "User Name is already registerd" };

			//TODO:USE Automapper
			var user = new ApplicationUser
			{
				UserName = model.UserName,
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
			};

			var result=await _usermanger.CreateAsync(user,model.Password);
			
			
			if (!result.Succeeded)
			{
				string errors = string.Empty;
				foreach (var error in result.Errors)
				{
					errors += $"{error.Description},";
				}
				return new AuthModel { Message = errors};
			}
			await _usermanger.AddToRoleAsync(user, "User");

			var jwtSecurityTokens = await CreateToken(user);

			return new AuthModel
			{
				Email = user.Email,
				ExpireOn = jwtSecurityTokens.ValidTo,
				IsAuthenticated = true,
				Roles = new List<string> { "User" },
				Token=new JwtSecurityTokenHandler().WriteToken(jwtSecurityTokens),
				UserName=user.UserName
			};
		}

		public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
		{
			var authModel=new AuthModel();
			var user=await _usermanger.FindByEmailAsync(model.Email);
			if (user is null|| !await _usermanger.CheckPasswordAsync(user, model.Password))
			{
				authModel.Message = "Email or Password is incorrect";
				return authModel;
			}
			var jwtSecurityTokens = await CreateToken(user);
			var roleslist = await _usermanger.GetRolesAsync(user);

			authModel.IsAuthenticated = true;
			authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityTokens);

			authModel.Email = user.Email;
			authModel.UserName = user.UserName;
			authModel.ExpireOn = jwtSecurityTokens.ValidTo;
			authModel.Roles = roleslist.ToList();


			return authModel;
		}

		private async Task<JwtSecurityToken>CreateToken(ApplicationUser user)
		{
			var UserClaims=await _usermanger.GetClaimsAsync(user);
			var roles=await _usermanger.GetRolesAsync(user);
			var roleClaims = new List<Claim>();

			foreach (var role in roles)
			{
				roleClaims.Add(new Claim("roles", role));
			}

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub,user.Id),
				new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Email,user.Email),
				new Claim(ClaimTypes.NameIdentifier,user.Id)
			}
			.Union(UserClaims)
			.Union(roleClaims);

			var symetricSecuritykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
			var signingcredtials = new SigningCredentials(symetricSecuritykey, SecurityAlgorithms.HmacSha256);

			var JwtSecurityToken = new JwtSecurityToken(
				issuer: _jwt.Issuer,
				audience: _jwt.Audience,
				claims: claims,
				expires: DateTime.Now.AddDays(_jwt.DurationInDays),
				signingCredentials: signingcredtials
				) ;
			return JwtSecurityToken;
		}

		public async Task<AuthModel> AddRoleAsync(AddRoleModel model)
		{
			var authModel = new AuthModel();
			// Check if the role exists
			if (!await _roleManger.RoleExistsAsync(model.RoleName))
			{
				authModel.Message = "Role does not exist.";
				return authModel;
			}

			// Find the user by ID
			var user = await _usermanger.FindByIdAsync(model.UserId);
			if (user == null)
			{
				authModel.Message = "User not found.";
				return authModel;
			}

			// Add the role to the user
			var result = await _usermanger.AddToRoleAsync(user, model.RoleName);
			if (!result.Succeeded)
			{
				authModel.Message = "Failed to add role.";
				return authModel;
			}

			// Generate a new token for the user with the updated roles
			var jwtSecurityTokens = await CreateToken(user);
			var roleslist = await _usermanger.GetRolesAsync(user);

			authModel.IsAuthenticated = true;
			authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityTokens);

			authModel.Email = user.Email;
			authModel.UserName = user.UserName;
			authModel.ExpireOn = jwtSecurityTokens.ValidTo;
			authModel.Roles = roleslist.ToList();

			return authModel;
		}

	}
}
