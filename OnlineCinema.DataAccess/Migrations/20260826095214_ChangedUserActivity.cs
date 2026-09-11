using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinema.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedUserActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("UserActivities");

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    EntityId = table.Column<string>(nullable: true),
                    EntityType = table.Column<string>(nullable: false),
                    ActionType = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Metadata = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                    table.ForeignKey("FK_UserActivities_Users_UserId", x => x.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserActivities_Movies_MovieId",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "UserActivities");

            migrationBuilder.AlterColumn<Guid>(
                name: "MovieId",
                table: "UserActivities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivities_Movies_MovieId",
                table: "UserActivities",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
