using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garagem75.Api.Migrations
{
    /// <inheritdoc />
    public partial class newUsu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "IdUsuario", "Ativo", "Email", "Nome", "Senha", "TipoUsuarioId" },
                values: new object[] { 1, true, "ronaldo@email.com", "Ronaldo", "123", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuario",
                keyColumn: "IdUsuario",
                keyValue: 1);
        }
    }
}
