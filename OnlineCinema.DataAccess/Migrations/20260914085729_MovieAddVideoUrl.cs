using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinema.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MovieAddVideoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Movies",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Movies");
        }
    }
}
