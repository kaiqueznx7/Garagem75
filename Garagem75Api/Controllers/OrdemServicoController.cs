using AutoMapper;
using Garagem75.Api.Data;
using Garagem75.Shared.Dtos;
using Garagem75.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Garagem75.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdemServicoController : ControllerBase
    {
        private readonly Garagem75DBContext _context;
        private readonly IMapper _mapper;

        public OrdemServicoController(Garagem75DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET
        // GET: api/OrdemServico
        [HttpGet]
        public async Task<ActionResult<List<OrdemServicoDto>>> GetAll()
        {
            try
            {
                // Traz as ordens do banco sem carregar grafos complexos via Include
                var ordens = await _context.OrdemServicos
                    .AsNoTracking()
                    .OrderByDescending(o => o.DataServico)
                    .Select(o => new OrdemServicoDto
                    {
                        IdOrdemServico = o.IdOrdemServico,
                        Descricao = o.Descricao,
                        Status = o.Status,
                        MaoDeObra = o.MaoDeObra,
                        ValorDesconto = o.ValorDesconto,
                        ValorTotal = o.ValorTotal,
                        DataServico = o.DataServico,
                        DataEntrega = o.DataEntrega,
                        VeiculoId = o.VeiculoId,

                        // Busca dados do Veiculo de forma defensiva se existir relacionamento
                        PlacaVeiculo = o.Veiculo != null ? o.Veiculo.Placa : "Sem veículo",
                        NomeCliente = (o.Veiculo != null && o.Veiculo.Cliente != null)
                            ? o.Veiculo.Cliente.Nome
                            : "Sem cliente",
                        ClienteId = o.Veiculo != null && o.Veiculo.ClienteId.HasValue
                            ? o.Veiculo.ClienteId.Value
                            : 0
                    })
                    .ToListAsync();

                return Ok(ordens);
            }
            catch (Exception ex)
            {
                // Exibe o erro exato no console
                Console.WriteLine("====================================");
                Console.WriteLine($"[ERRO DETALHADO OS GETALL]: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[INNER EXCEPTION]: {ex.InnerException.Message}");
                }
                Console.WriteLine("====================================");

                return StatusCode(500, $"Erro ao buscar OS: {ex.Message}");
            }
        }
        // POST
        [HttpPost]
        public async Task<ActionResult<OrdemServicoDto>> Create([FromBody] OrdemServicoDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos.");

                // Garante que existe um VeiculoId válido
                if (dto.VeiculoId <= 0)
                    return BadRequest("Selecione um veículo válido.");

                var novaOs = new OrdemServico
                {
                    VeiculoId = dto.VeiculoId,
                    Descricao = dto.Descricao ?? "Sem descrição",
                    Status = string.IsNullOrWhiteSpace(dto.Status) ? "Aberta" : dto.Status,
                    MaoDeObra = dto.MaoDeObra,
                    ValorDesconto = dto.ValorDesconto,
                    ValorTotal = dto.ValorTotal,
                    DataServico = dto.DataServico != default ? dto.DataServico : DateTime.Now,
                    DataEntrega = dto.DataEntrega,

                    // IMPORTANTE: Zera navegações para o EF não tentar criar registros duplicados nas tabelas pai
                    Veiculo = null
                };

                _context.OrdemServicos.Add(novaOs);
                await _context.SaveChangesAsync();

                dto.IdOrdemServico = novaOs.IdOrdemServico;

                return Created($"api/ordemservico/{novaOs.IdOrdemServico}", dto);
            }
            catch (Exception ex)
            {
                // Pega a mensagem mais profunda do SQL Server
                var mensagemErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                System.Diagnostics.Debug.WriteLine($"[ERRO CRÍTICO SQL]: {mensagemErro}");

                // Retorna o detalhe direto no Swagger / DevTools para vermos o texto exato
                return StatusCode(500, $"Erro no Banco: {mensagemErro}");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrdemServicoDto dto)
        {
            if (id != dto.IdOrdemServico) return BadRequest();

            // 1. Carrega a OS com as peças atuais
            var entity = await _context.OrdemServicos
                .Include(x => x.PecasAssociadas)
                .FirstOrDefaultAsync(x => x.IdOrdemServico == id);

            if (entity == null) return NotFound();

            // 2. Mapeia os dados básicos (Descricao, MaoDeObra, etc)
            _mapper.Map(dto, entity);

            // 3. GERENCIAMENTO DE PEÇAS (Para o MVC funcionar)
            // Se o DTO trouxe uma lista de peças, vamos sincronizar com o banco
            if (dto.PecasAssociadas != null)
            {
                // Remove o que não está mais no DTO ou limpa tudo para reinserir
                _context.OrdemServicoPecas.RemoveRange(entity.PecasAssociadas);

                foreach (var p in dto.PecasAssociadas)
                {
                    // Busca o preço atual da peça para o cálculo ser real
                    var pecaDb = await _context.Pecas.AsNoTracking().FirstOrDefaultAsync(x => x.IdPeca == p.PecaId);

                    entity.PecasAssociadas.Add(new OrdemServicoPeca
                    {
                        OrdemServicoId = id,
                        PecaId = p.PecaId,
                        Quantidade = p.Quantidade,
                        PrecoUnitario = pecaDb?.Preco ?? 0
                    });
                }
            }

            // 4. RECALCULA O TOTAL GERAL (A ÚNICA FONTE DE VERDADE)
            decimal totalPecas = entity.PecasAssociadas.Sum(p => p.Quantidade * p.PrecoUnitario);
            entity.ValorTotal = (entity.MaoDeObra + totalPecas) - entity.ValorDesconto;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar OS {id}: {ex.Message}");
                return BadRequest("Erro ao atualizar os dados no banco.");
            }
        }
        // FINALIZAR
        [HttpPut("{id}/finalizar")]
        public async Task<IActionResult> Finalizar(int id)
        {
            var os = await _context.OrdemServicos.FindAsync(id);

            if (os == null)
                return NotFound();

            os.Status = "Finalizada";

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.OrdemServicos.FindAsync(id);

            if (entity == null)
                return NotFound();

            _context.OrdemServicos.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}