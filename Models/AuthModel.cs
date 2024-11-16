namespace MoviesApi.Models
{
	public class AuthModel
	{
		public string Message { get; set; } = string.Empty;
		public bool IsAuthenticated { get; set; }
		public string UserName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public List<string> Roles { get; set; } = new List<string>();
		public string Token { get; set; } = string.Empty;
		public DateTime ExpireOn { get; set; }
	}
}
