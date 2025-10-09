using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garagem75.Migrations
{
    /// <inheritdoc />
    public partial class pecaOrdem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdemServicoPeca",
                columns: table => new
                {
                    OrdemServicosIdOrdemServico = table.Column<int>(type: "int", nullable: false),
                    PecasIdPeca = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdemServicoPeca", x => new { x.OrdemServicosIdOrdemServico, x.PecasIdPeca });
                    table.ForeignKey(
                        name: "FK_OrdemServicoPeca_OrdemServicos_OrdemServicosIdOrdemServico",
                        column: x => x.OrdemServicosIdOrdemServico,
                        principalTable: "OrdemServicos",
                        principalColumn: "IdOrdemServico",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdemServicoPeca_Peca_PecasIdPeca",
                        column: x => x.PecasIdPeca,
                        principalTable: "Peca",
                        principalColumn: "IdPeca",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicoPeca_PecasIdPeca",
                table: "OrdemServicoPeca",
                column: "PecasIdPeca");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdemServicoPeca");
        }
    }
}
