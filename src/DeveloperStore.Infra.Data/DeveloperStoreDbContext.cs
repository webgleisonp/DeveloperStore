using DeveloperStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Infra.Data;

public sealed class DeveloperStoreDbContext : DbContext
{
    public DeveloperStoreDbContext(DbContextOptions<DeveloperStoreDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aplicar todas as configurações de entidades do assembly atual
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeveloperStoreDbContext).Assembly);
        
        // Definir o schema padrão
        modelBuilder.HasDefaultSchema("public");
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Configurar convenções globais
        configurationBuilder
            .Properties<string>()
            .AreUnicode(false)
            .HaveMaxLength(255);
    }
}
