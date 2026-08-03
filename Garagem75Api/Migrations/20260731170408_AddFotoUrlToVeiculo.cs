using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garagem75.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoUrlToVeiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
    name: "FotoUrl",
    table: "Veiculos",
    type: "nvarchar(max)",
    nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
