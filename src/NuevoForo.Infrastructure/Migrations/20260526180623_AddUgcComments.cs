using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuevoForo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUgcComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ResenaId",
                table: "Comentarios",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "UgcId",
                table: "Comentarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UgcId",
                table: "Comentarios",
                column: "UgcId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_ContenidosUgc_UgcId",
                table: "Comentarios",
                column: "UgcId",
                principalTable: "ContenidosUgc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_ContenidosUgc_UgcId",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_UgcId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "UgcId",
                table: "Comentarios");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResenaId",
                table: "Comentarios",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
