using AutoMapper;
using Garagem75.Api.Data;
using Garagem75.Shared; // <-- IMPORTANTE
using Garagem75.Shared.Dtos;
using Garagem75.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Garagem75.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Administrador,Mecânico")]
    public class VeiculoController : ControllerBase
    {
        private readonly Garagem75DBContext _context;

        private readonly IMapper _mapper;

        public VeiculoController(Garagem75DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // GET: api/veiculo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VeiculoDto>>> GetAll(
            string? searchPlaca,
            string? searchCliente)
        {
            try
            {
                var query = _context.Veiculos
                    .Include(v => v.Cliente)
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchPlaca))
                    query = query.Where(v => v.Placa != null && v.Placa.Contains(searchPlaca));

                if (!string.IsNullOrWhiteSpace(searchCliente))
                    query = query.Where(v => v.Cliente != null && v.Cliente.Nome != null && v.Cliente.Nome.Contains(searchCliente));

                var lista = await query.ToListAsync();

                // Mapeamento via AutoMapper envelopado em tratamento de erro
                var dtos = _mapper.Map<List<VeiculoDto>>(lista);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO GET ALL VEICULOS]: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[INNER EXCEPTION]: {ex.InnerException.Message}");
                }

                // Evita crash de 500 no backend retornando 200 OK com lista vazia em caso de falha de mapeamento
                return Ok(new List<VeiculoDto>());
            }
        }

        // GET BY ID: api/veiculo/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<VeiculoDto>> GetById(int id)
        {
            try
            {
                var v = await _context.Veiculos
                    .Include(x => x.Cliente)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdVeiculo == id);

                if (v == null)
                    return NotFound();

                return Ok(_mapper.Map<VeiculoDto>(v));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO GET VEICULO BY ID]: {ex.Message}");
                return StatusCode(500, "Erro interno ao buscar veículo.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<VeiculoDto>> Create([FromBody] VeiculoDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { mensagem = "Dados inválidos." });

                if (dto.ClienteId <= 0)
                    return BadRequest(new { mensagem = "Selecione um cliente válido." });

                // Instanciação manual da Entidade para EVITAR que o AutoMapper force IdVeiculo = 0 ou envie a navegação Cliente
                var veiculo = new Veiculo
                {
                    Fabricante = dto.Fabricante ?? "",
                    Modelo = dto.Modelo ?? "",
                    Ano = dto.Ano,
                    Placa = dto.Placa ?? "",
                    Cor = dto.Cor ?? "",
                    FotoUrl = dto.FotoUrl,
                    ClienteId = dto.ClienteId // Vincula a Foreign Key do Cliente
                };

                _context.Veiculos.Add(veiculo);
                await _context.SaveChangesAsync();

                // Atualiza o ID gerado no DTO de retorno
                dto.IdVeiculo = veiculo.IdVeiculo;

                return CreatedAtAction(nameof(GetById), new { id = veiculo.IdVeiculo }, dto);
            }
            catch (Exception ex)
            {
                var innerError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"[ERRO EXATO BANCO]: {innerError}");

                return StatusCode(500, new
                {
                    mensagem = $"Erro no banco: {innerError}"
                });
            }
        }
        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, VeiculoDto dto)
        {
            if (id != dto.IdVeiculo)
                return BadRequest();

            var veiculo = await _context.Veiculos.FindAsync(id);

            if (veiculo == null)
                return NotFound();
            // ✅ Verifica placa duplicada, ignorando o próprio veículo
            bool placaExiste = await _context.Veiculos
                .AnyAsync(v => v.Placa == dto.Placa && v.IdVeiculo != id);

            if (placaExiste)
                return BadRequest(new { mensagem = "Placa já cadastrada." });

            // Atualiza campos
            _mapper.Map(dto, veiculo);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);

            if (veiculo == null)
                return NotFound();

            _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<Veiculo>>> GetByCliente(int clienteId)
        {
            var veiculos = await _context.Veiculos
                .Where(v => v.ClienteId == clienteId)
                .ToListAsync();

            return Ok(veiculos);
        }
    }
}