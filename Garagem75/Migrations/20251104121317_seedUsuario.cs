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
            INSERT INTO Usuario (Nome, Cpf, Telefone, Email)
            VALUES ('Beto Barba', '34869389835', '(11)981203489', 'beto@teste.com');
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
