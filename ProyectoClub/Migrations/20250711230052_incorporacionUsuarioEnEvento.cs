using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoClub.Migrations
{
    /// <inheritdoc />
    public partial class incorporacionUsuarioEnEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Eventos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_UsuarioId",
                table: "Eventos",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_AspNetUsers_UsuarioId",
                table: "Eventos",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_AspNetUsers_UsuarioId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_UsuarioId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Eventos");
        }
    }
}
