using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garagem75.Migrations
{
    /// <inheritdoc />
    public partial class qtdpeca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdemServicoPeca");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorDesconto",
                table: "OrdemServicos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "OrdemServicoPecas",
                columns: table => new
                {
                    OrdemServicoId = table.Column<int>(type: "int", nullable: false),
                    PecaId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PecaIdPeca = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdemServicoPecas", x => new { x.OrdemServicoId, x.PecaId });
                    table.ForeignKey(
                        name: "FK_OrdemServicoPecas_OrdemServicos_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdemServicos",
                        principalColumn: "IdOrdemServico",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdemServicoPecas_Peca_PecaId",
                        column: x => x.PecaId,
                        principalTable: "Peca",
                        principalColumn: "IdPeca",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdemServicoPecas_Peca_PecaIdPeca",
                        column: x => x.PecaIdPeca,
                        principalTable: "Peca",
                        principalColumn: "IdPeca");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicoPecas_PecaId",
                table: "OrdemServicoPecas",
                column: "PecaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicoPecas_PecaIdPeca",
                table: "OrdemServicoPecas",
                column: "PecaIdPeca");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdemServicoPecas");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorDesconto",
                table: "OrdemServicos",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

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
    }
}
