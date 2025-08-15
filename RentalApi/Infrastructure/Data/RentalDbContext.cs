using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;

namespace RentalApi.Infrastructure.Data
{
    public class RentalDbContext : DbContext
    {
        public RentalDbContext(DbContextOptions<RentalDbContext> options) : base(options) { }
        public DbSet<Moto> Motos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Moto>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasIndex(m => m.Identificador).IsUnique();
                entity.Property(m => m.Identificador).IsRequired().HasMaxLength(50);
                entity.HasIndex(m => m.Placa).IsUnique();
                entity.Property(m => m.Placa).IsRequired().HasMaxLength(10);
                entity.Property(m => m.Modelo).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Ano).IsRequired();
            });
            modelBuilder.Entity<Moto>().HasData(
                new Moto { Id = 1, Identificador = "moto123", Ano = 2020, Modelo = "Mottu Sport", Placa = "CDX-0101" }
            );
        }
    }
}