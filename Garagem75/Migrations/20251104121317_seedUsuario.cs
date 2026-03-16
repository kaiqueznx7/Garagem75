using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garagem75.Migrations
{
    /// <inheritdoc />
    public partial class seedUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            INSERT INTO Usuario (Nome, Email, Senha, TipoUsuarioId, Ativo)
            VALUES ('Beto Barba', 'beto2@teste.com', '123456', '1', 'true')
    ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DELETE FROM Usuario WHERE Email = 'beto@teste.com';
    ");

        }
    }
}
