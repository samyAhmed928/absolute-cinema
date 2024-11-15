using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using MoviesApi.Models;

namespace MoviesApi.Data
{
    public class ApplictionDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplictionDbContext(DbContextOptions<ApplictionDbContext> options) : base(options) {}

        public DbSet<Genre> Genres {  get; set; }
        public DbSet<Movie> Movies {  get; set; }

    }
}
