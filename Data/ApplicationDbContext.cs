using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using MoviesApi.Models;
using System.Reflection.Emit;

namespace MoviesApi.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        public DbSet<Genre> Genres {  get; set; }
        public DbSet<Movie> Movies {  get; set; }
		public DbSet<MovieRating> MovieRatings { get; set; }
		public DbSet<MovieReview> MovieReview { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
            base.OnModelCreating(builder);

			// One-to-Many: Admin -> Movies
			builder.Entity<Movie>()
				.HasOne(m => m.Admin)
				.WithMany()
				.HasForeignKey(m => m.AdminId)
				.OnDelete(DeleteBehavior.Restrict);
			builder.Entity<MovieRating>().HasKey(x => new { x.MovieId, x.UserId });
			// Many-to-Many: Users <-> Movies via MovieRating
			builder.Entity<MovieRating>()
				.HasOne(mr => mr.User)
				.WithMany()
				.HasForeignKey(mr => mr.UserId);

			builder.Entity<MovieRating>()
				.HasOne(mr => mr.Movie)
				.WithMany(m => m.movieRatings)
				.HasForeignKey(mr => mr.MovieId);

			builder.Entity<MovieReview>()
				.HasOne(mr => mr.User)
				.WithMany()
				.HasForeignKey(mr => mr.UserId);

			builder.Entity<MovieReview>()
				.HasOne(mr => mr.Movie)
				.WithMany(m => m.MovieReviews)
				.HasForeignKey(mr => mr.MovieId);

		}
	}
}
