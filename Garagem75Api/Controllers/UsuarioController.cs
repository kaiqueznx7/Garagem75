using AutoMapper;
using Garagem75.Api.Data;
using Garagem75.Shared.Dtos;
using Garagem75.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Garagem75.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly Garagem75DBContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsuarioController(Garagem75DBContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
        {
            var lista = await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .ToListAsync();
            Console.WriteLine($"TOTAL DE USUARIOS NO BANCO: {lista.Count}");

            return Ok(_mapper.Map<List<UsuarioDto>>(lista));
        }

        [Authorize(Roles = "Administrador")]

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> GetById(int id)
        {
            var item = await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .Where(u => u.Ativo)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (item == null)
                return NotFound();

            return Ok(_mapper.Map<UsuarioDto>(item));
        }
        [Authorize(Roles = "Administrador")]

        [HttpPost]
        public async Task<ActionResult> Create(UsuarioDto dto)
        {
            // ✅ Verifica e-mail duplicado
            bool emailExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email);

            if (emailExiste)
                return BadRequest(new { mensagem = "E-mail já cadastrado." });
            var entity = _mapper.Map<Usuario>(dto);

            _context.Usuarios.Add(entity);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<UsuarioDto>(entity);

            return CreatedAtAction(nameof(GetById),
                new { id = result.IdUsuario }, result);
        }
        [Authorize(Roles = "Administrador")]

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UsuarioDto dto)
        {

            if (id != dto.IdUsuario)
                return BadRequest();

            var entity = await _context.Usuarios.FindAsync(id);

            if (entity == null)
                return NotFound();
            // ✅ Verifica e-mail duplicado, ignorando o próprio usuário
            bool emailExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email && u.IdUsuario != id);

            if (emailExiste)
                return BadRequest(new { mensagem = "E-mail já cadastrado." });

            _mapper.Map(dto, entity);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        [Authorize(Roles = "Administrador")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Usuarios.FindAsync(id);

            if (entity == null)
                return NotFound();

            entity.Ativo = false; // 🔥 NÃO DELETA MAIS

            await _context.SaveChangesAsync();

            return NoContent();
        }
        [Authorize(Roles = "Administrador")]

        [HttpPut("{id}/reativar")]
        public async Task<IActionResult> Reativar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            usuario.Ativo = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                Console.WriteLine($"[LOGIN] Tentando autenticar EMAIL: '{login.Email}'");

                // 1. Busca pelo e-mail
                var user = await _context.Usuarios
                    .Include(u => u.TipoUsuario)
                    .FirstOrDefaultAsync(u => u.Email.ToLower().Trim() == login.Email.ToLower().Trim());

                if (user == null)
                    return Unauthorized("Usuário não encontrado no banco.");

                // 2. Valida a senha
                if (user.Senha.Trim() != login.Senha.Trim())
                    return Unauthorized("Senha incorreta.");

                // 3. Valida se está ativo
                if (!user.Ativo)
                    return Unauthorized("Usuário inativo.");

                // 4. Validação e Leitura da Chave JWT
                var chaveJwt = _configuration["Jwt:ChaveSecreta"];
                if (string.IsNullOrEmpty(chaveJwt))
                {
                    Console.WriteLine("🔥 [ERRO CRÍTICO]: A chave 'Jwt:ChaveSecreta' não foi encontrada no appsettings.json!");
                    return StatusCode(500, "Erro interno de configuração de autenticação.");
                }

                if (chaveJwt.Length < 32)
                {
                    Console.WriteLine("🔥 [ERRO CRÍTICO]: A chave JWT deve ter pelo menos 32 caracteres.");
                    return StatusCode(500, "Erro interno de configuração de chave de segurança.");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
            new Claim(ClaimTypes.Name, user.Nome ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Role, user.TipoUsuario?.DescricaoTipoUsuario ?? "Membro")
        };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    token = tokenString,
                    nome = user.Nome,
                    tipo = user.TipoUsuario?.DescricaoTipoUsuario ?? "Membro"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 [ERRO NO LOGIN]: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"🔥 [DETALHE INTERNO]: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Falha interna ao processar o login: {ex.Message}");
            }
        }
    }
}