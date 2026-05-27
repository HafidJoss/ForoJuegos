using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuevoForo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDislikeToLikeResena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsDislike",
                table: "LikesResena",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsDislike",
                table: "LikesResena");
        }
    }
}
