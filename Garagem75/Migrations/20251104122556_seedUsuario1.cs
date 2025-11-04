using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garagem75.Migrations
{
    /// <inheritdoc />
    public partial class seedUsuario1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            INSERT INTO Usuario (Nome, Email, Senha, TipoUsuarioId, Ativo)
            VALUES ('Beto Barba', 'beto@teste.com', '123bet', 1, 1);
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
