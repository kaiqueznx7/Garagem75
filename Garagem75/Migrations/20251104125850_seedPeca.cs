using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garagem75.Migrations
{
    /// <inheritdoc />
    public partial class seedPeca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                INSERT INTO Peca     (CodPeca, Marca, Nome, Preco, Fornecedor, QuantidadeEstoque, DataCadastro, DataUltimaAtualizacao, Imagem) VALUES
                (1001, 'Lubrax', 'Óleo de Motor 5W30 Sintético', 79.90, 'Auto Peças Brasil', 50, GETDATE(), GETDATE(), 'oleo_motor.jpg'),
                (1002, 'Monroe', 'Amortecedor Dianteiro', 320.00, 'Centro Automotivo Nacional', 20, GETDATE(), GETDATE(), 'amortecedor.jpg'),
                (1003, 'Bosch', 'Bomba d''Água', 249.50, 'Auto Peças Brasil', 15, GETDATE(), GETDATE(), 'bomba_agua.jpg'),
                (1004, 'Fram', 'Filtro de Óleo', 39.90, 'Distribuidora AutoParts', 80, GETDATE(), GETDATE(), 'filtro_oleo.jpg'),
                (1005, 'Mahle', 'Filtro de Ar', 49.00, 'Distribuidora AutoParts', 60, GETDATE(), GETDATE(), 'filtro_ar.jpg'),
                (1006, 'NGK', 'Vela de Ignição', 35.00, 'Centro Automotivo Nacional', 100, GETDATE(), GETDATE(), 'vela_ignicao.jpg'),
                (1007, 'Cobreq', 'Pastilha de Freio Dianteira', 199.90, 'Freios & Cia', 30, GETDATE(), GETDATE(), 'pastilha_freio.jpg'),
                (1008, 'Valeo', 'Embreagem Completa', 850.00, 'Auto Peças Brasil', 10, GETDATE(), GETDATE(), 'embreagem.jpg'),
                (1009, 'Dayco', 'Correia Dentada', 120.00, 'Distribuidora AutoParts', 40, GETDATE(), GETDATE(), 'correia_dentada.jpg'),
                (1010, 'Bosch', 'Filtro de Combustível', 59.90, 'Distribuidora AutoParts', 70, GETDATE(), GETDATE(), 'filtro_combustivel.jpg');
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                DELETE FROM Pecas WHERE CodPeca BETWEEN 1001 AND 1010;
            ");

        }
    }
}
