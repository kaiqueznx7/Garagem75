using Microsoft.EntityFrameworkCore;

namespace Garagem75.Data;

    public class Garagem75DBContext:DbContext
    {
     public Garagem75DBContext(DbContextOptions<Garagem75DBContext> options) : base(options)
    {
    }
    public DbSet<Models.Veiculo> Veiculos { get; set; }
    public DbSet<Models.Usuario> Usuarios { get; set; }
    public DbSet<Models.Peca> Pecas { get; set; }
    public DbSet<Models.TipoUsuario> TipoUsuarios { get; set; }
    public DbSet<Models.Cliente> Clientes { get; set; }
    public DbSet<Models.Endereco> Enderecos { get; set; }
    public DbSet<Models.OrdemServico> OrdemServicos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configurações adicionais podem ser feitas aqui
        modelBuilder.Entity<Models.Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}

