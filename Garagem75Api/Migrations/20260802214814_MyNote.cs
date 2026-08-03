using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garagem75.Api.Migrations
{
    /// <inheritdoc />
    public partial class MyNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Cliente_ClienteId",
                table: "OrdemServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiculo_Cliente_ClienteId",
                table: "Veiculo");

            migrationBuilder.DropIndex(
                name: "IX_OrdemServicos_ClienteId",
                table: "OrdemServicos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "OrdemServicos");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Veiculo",
                newName: "IdCliente");

            migrationBuilder.RenameIndex(
                name: "IX_Veiculo_ClienteId",
                table: "Veiculo",
                newName: "IX_Veiculo_IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculo_Cliente_IdCliente",
                table: "Veiculo",
                column: "IdCliente",
                principalTable: "Cliente",
                principalColumn: "IdCliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiculo_Cliente_IdCliente",
                table: "Veiculo");

            migrationBuilder.RenameColumn(
                name: "IdCliente",
                table: "Veiculo",
                newName: "ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Veiculo_IdCliente",
                table: "Veiculo",
                newName: "IX_Veiculo_ClienteId");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "OrdemServicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicos_ClienteId",
                table: "OrdemServicos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Cliente_ClienteId",
                table: "OrdemServicos",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculo_Cliente_ClienteId",
                table: "Veiculo",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "IdCliente");
        }
    }
}
