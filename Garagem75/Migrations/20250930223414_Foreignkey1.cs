using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garagem75.Migrations
{
    /// <inheritdoc />
    public partial class Foreignkey1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endereco_Cliente_ClienteIdCliente",
                table: "Endereco");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Veiculo_VeiculoIdVeiculo",
                table: "OrdemServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiculo_Cliente_ClienteIdCliente",
                table: "Veiculo");

            migrationBuilder.DropIndex(
                name: "IX_OrdemServicos_VeiculoIdVeiculo",
                table: "OrdemServicos");

            migrationBuilder.DropColumn(
                name: "VeiculoIdVeiculo",
                table: "OrdemServicos");

            migrationBuilder.RenameColumn(
                name: "ClienteIdCliente",
                table: "Veiculo",
                newName: "ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Veiculo_ClienteIdCliente",
                table: "Veiculo",
                newName: "IX_Veiculo_ClienteId");

            migrationBuilder.RenameColumn(
                name: "ClienteIdCliente",
                table: "Endereco",
                newName: "ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Endereco_ClienteIdCliente",
                table: "Endereco",
                newName: "IX_Endereco_ClienteId");

            migrationBuilder.AddColumn<int>(
                name: "VeiculoId",
                table: "OrdemServicos",
                type: "int",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicos_VeiculoId",
                table: "OrdemServicos",
                column: "VeiculoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Endereco_Cliente_ClienteId",
                table: "Endereco",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Veiculo_VeiculoId",
                table: "OrdemServicos",
                column: "VeiculoId",
                principalTable: "Veiculo",
                principalColumn: "IdVeiculo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculo_Cliente_ClienteId",
                table: "Veiculo",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "IdCliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endereco_Cliente_ClienteId",
                table: "Endereco");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Veiculo_VeiculoId",
                table: "OrdemServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiculo_Cliente_ClienteId",
                table: "Veiculo");

            migrationBuilder.DropIndex(
                name: "IX_OrdemServicos_VeiculoId",
                table: "OrdemServicos");

            migrationBuilder.DropColumn(
                name: "VeiculoId",
                table: "OrdemServicos");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Veiculo",
                newName: "ClienteIdCliente");

            migrationBuilder.RenameIndex(
                name: "IX_Veiculo_ClienteId",
                table: "Veiculo",
                newName: "IX_Veiculo_ClienteIdCliente");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Endereco",
                newName: "ClienteIdCliente");

            migrationBuilder.RenameIndex(
                name: "IX_Endereco_ClienteId",
                table: "Endereco",
                newName: "IX_Endereco_ClienteIdCliente");

            migrationBuilder.AddColumn<int>(
                name: "VeiculoIdVeiculo",
                table: "OrdemServicos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicos_VeiculoIdVeiculo",
                table: "OrdemServicos",
                column: "VeiculoIdVeiculo");

            migrationBuilder.AddForeignKey(
                name: "FK_Endereco_Cliente_ClienteIdCliente",
                table: "Endereco",
                column: "ClienteIdCliente",
                principalTable: "Cliente",
                principalColumn: "IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Veiculo_VeiculoIdVeiculo",
                table: "OrdemServicos",
                column: "VeiculoIdVeiculo",
                principalTable: "Veiculo",
                principalColumn: "IdVeiculo");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculo_Cliente_ClienteIdCliente",
                table: "Veiculo",
                column: "ClienteIdCliente",
                principalTable: "Cliente",
                principalColumn: "IdCliente");
        }
    }
}
