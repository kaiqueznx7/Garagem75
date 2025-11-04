using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garagem75.Migrations
{
    /// <inheritdoc />
    public partial class seedTipoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        INSERT INTO TipoUsuario (DescricaoTipoUsuario)
        VALUES ('Administrador'),('Mecânico');

        
    ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DELETE FROM TipoUsuario WHERE IdTipoUsuario IN (1, 2);
    ");

        }
    }
}
