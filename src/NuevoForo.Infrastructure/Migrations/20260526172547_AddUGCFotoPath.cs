using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuevoForo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUGCFotoPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContenidosUgc_Juegos_JuegoId",
                table: "ContenidosUgc");

            migrationBuilder.AlterColumn<Guid>(
                name: "JuegoId",
                table: "ContenidosUgc",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "FotoNombre",
                table: "ContenidosUgc",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoPath",
                table: "ContenidosUgc",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FotoSize",
                table: "ContenidosUgc",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContenidosUgc_Juegos_JuegoId",
                table: "ContenidosUgc",
                column: "JuegoId",
                principalTable: "Juegos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContenidosUgc_Juegos_JuegoId",
                table: "ContenidosUgc");

            migrationBuilder.DropColumn(
                name: "FotoNombre",
                table: "ContenidosUgc");

            migrationBuilder.DropColumn(
                name: "FotoPath",
                table: "ContenidosUgc");

            migrationBuilder.DropColumn(
                name: "FotoSize",
                table: "ContenidosUgc");

            migrationBuilder.AlterColumn<Guid>(
                name: "JuegoId",
                table: "ContenidosUgc",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContenidosUgc_Juegos_JuegoId",
                table: "ContenidosUgc",
                column: "JuegoId",
                principalTable: "Juegos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
