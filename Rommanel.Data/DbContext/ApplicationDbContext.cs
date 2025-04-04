using Microsoft.EntityFrameworkCore;
using Rommanel.Domain.Entities;

namespace Rommanel.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CPFCNPJ)
                    .IsRequired()
                    .HasMaxLength(14);

                entity.Property(e => e.IE)
                    .HasMaxLength(12);

                entity.Property(e => e.Telefone)
                    .HasMaxLength(20);

                entity.Property(e => e.Email)
                    .HasMaxLength(200);

                entity.Property(e => e.Cep)
                    .HasMaxLength(8);

                entity.Property(e => e.Logradouro)
                    .HasMaxLength(100);

                entity.Property(e => e.Numero)
                    .HasMaxLength(10);

                entity.Property(e => e.Complemento)
                    .HasMaxLength(50);

                entity.Property(e => e.Bairro)
                    .HasMaxLength(100);

                entity.Property(e => e.Cidade)
                    .HasMaxLength(100);

                entity.Property(e => e.Estado)
                    .HasMaxLength(2);

                entity.Property(e => e.Tipo)
                    .HasConversion<int>();

                entity.HasIndex(e => e.CPFCNPJ).IsUnique();
                entity.HasIndex(e => e.Email);
            });
        }
    }
}