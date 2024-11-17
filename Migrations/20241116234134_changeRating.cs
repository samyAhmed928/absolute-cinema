using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesApi.Migrations
{
    /// <inheritdoc />
    public partial class changeRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieRatings",
                table: "MovieRatings");

            migrationBuilder.DropIndex(
                name: "IX_MovieRatings_MovieId",
                table: "MovieRatings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MovieRatings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieRatings",
                table: "MovieRatings",
                columns: new[] { "MovieId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieRatings",
                table: "MovieRatings");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MovieRatings",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieRatings",
                table: "MovieRatings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MovieRatings_MovieId",
                table: "MovieRatings",
                column: "MovieId");
        }
    }
}
