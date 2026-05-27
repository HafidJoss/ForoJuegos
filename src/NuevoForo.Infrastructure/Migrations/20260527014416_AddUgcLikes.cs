using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuevoForo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUgcLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LikesUgc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UgcId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    EsDislike = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ContenidoUgcId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikesUgc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikesUgc_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LikesUgc_ContenidosUgc_ContenidoUgcId",
                        column: x => x.ContenidoUgcId,
                        principalTable: "ContenidosUgc",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LikesUgc_ContenidosUgc_UgcId",
                        column: x => x.UgcId,
                        principalTable: "ContenidosUgc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikesUgc_ContenidoUgcId",
                table: "LikesUgc",
                column: "ContenidoUgcId");

            migrationBuilder.CreateIndex(
                name: "IX_LikesUgc_UgcId",
                table: "LikesUgc",
                column: "UgcId");

            migrationBuilder.CreateIndex(
                name: "IX_LikesUgc_UsuarioId_UgcId",
                table: "LikesUgc",
                columns: new[] { "UsuarioId", "UgcId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikesUgc");
        }
    }
}
