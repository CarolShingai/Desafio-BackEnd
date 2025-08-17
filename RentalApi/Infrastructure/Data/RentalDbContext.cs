using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;

namespace RentalApi.Infrastructure.Data
{
    /// <summary>
    /// Database context for the rental system, manages entity configurations and database access.
    /// </summary>
    public class RentalDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the RentalDbContext class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public RentalDbContext(DbContextOptions<RentalDbContext> options) : base(options) { }

        /// <summary>
        /// DbSet representing the motorcycles table.
        /// </summary>
        public DbSet<Moto> Motos { get; set; }
        public DbSet<MotoNotification> MotoNotifications { get; set; }

        /// <summary>
        /// Configures the entity mappings and seeds initial data.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the context.</param>
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
            modelBuilder.Entity<MotoNotification>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Message).IsRequired();
                entity.Property(n => n.NotifiedAt).IsRequired();
                entity.HasIndex(n => n.MotorcycleId);
            });
            // Seed initial data for Moto entity
            modelBuilder.Entity<Moto>().HasData(
                new Moto { Id = 1, Identificador = "moto123", Ano = 2020, Modelo = "Mottu Sport", Placa = "CDX-0101" }
            );
        }
    }
}