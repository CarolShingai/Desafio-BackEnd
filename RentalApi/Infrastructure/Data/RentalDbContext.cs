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
        public DbSet<DeliveryPerson> DeliveryPersons { get; set; }
        public DbSet<RentMoto> RentMotos { get; set; }

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

                entity.HasOne<Moto>()
                    .WithMany()
                    .HasForeignKey(n => n.MotorcycleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<DeliveryPerson>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.HasIndex(d => d.Identifier).IsUnique();
                entity.Property(d => d.Identifier).IsRequired().HasMaxLength(50);
                entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
                entity.Property(d => d.BirthDate).IsRequired().HasMaxLength(20);
                entity.HasIndex(d => d.Cnh).IsUnique();
                entity.Property(d => d.Cnh).IsRequired().HasMaxLength(11);
                entity.Property(d => d.CnhType).IsRequired().HasMaxLength(3);
                entity.Property(d => d.CnhImage).IsRequired();
                entity.HasIndex(d => d.Cnpj).IsUnique();
                entity.Property(d => d.Cnpj).IsRequired().HasMaxLength(14);
            });
            modelBuilder.Entity<RentMoto>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.HasIndex(r => r.RentId).IsUnique();
                entity.Property(r => r.RentId).IsRequired().HasMaxLength(50);
                entity.HasIndex(r => r.MotoId);
                entity.Property(r => r.StartDate).IsRequired();
                entity.Property(r => r.ExpectedReturnDate).IsRequired(false);
                entity.Property(r => r.ActualReturnDate).IsRequired(false);
                entity.Property(r => r.DeliveryPersonId).IsRequired();

                entity.HasOne<Moto>()
                    .WithMany()
                    .HasForeignKey(r => r.MotoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<DeliveryPerson>()
                    .WithMany()
                    .HasForeignKey(r => r.DeliveryPersonId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // Seed initial data for Moto entity
            modelBuilder.Entity<Moto>().HasData(
                new Moto { Id = 1, Identificador = "moto123", Ano = 2020, Modelo = "Mottu Sport", Placa = "CDX-0101", IsRented = true }
            );
            // modelBuilder.Entity<DeliveryPerson>().HasData(
            //     new DeliveryPerson
            //     {
            //         Id = 111111,
            //         Identifier = "entregador123",
            //         Name = "João Silva",
            //         Cnpj = "12345678901234",
            //         BirthDate = "1990-01-01",
            //         Cnh = "12345678901",
            //         CnhType = "A",
            //         CnhImage = "base64string"
            //     }
            // );
        }
    }
}