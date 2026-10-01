using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Data
{
    public class BibliotecaContext : IdentityDbContext<IdentityUser>
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options)
            : base(options)
        {
        }

        public DbSet<Libro> Libros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Libro>(entidad =>
            {
                entidad.ToTable("Libros");
                entidad.HasKey(l => l.Id);

                entidad.Property(l => l.Titulo).IsRequired().HasMaxLength(150);
                entidad.Property(l => l.Autor).IsRequired().HasMaxLength(100);
                entidad.Property(l => l.Editorial).HasMaxLength(100);
                entidad.Property(l => l.Isbn).HasMaxLength(20);
                entidad.Property(l => l.Ejemplares).HasDefaultValue(1);
                entidad.Property(l => l.Disponible).HasDefaultValue(true);

                // Registros iniciales del módulo de Libros.
                entidad.HasData(
                    new Libro
                    {
                        Id = 1,
                        Titulo = "Cien años de soledad",
                        Autor = "Gabriel García Márquez",
                        Editorial = "Sudamericana",
                        Isbn = "978-0307474728",
                        AnioPublicacion = 1967,
                        Ejemplares = 4,
                        Disponible = true,
                        FechaRegistro = new DateTime(2026, 1, 15)
                    },
                    new Libro
                    {
                        Id = 2,
                        Titulo = "Ficciones",
                        Autor = "Jorge Luis Borges",
                        Editorial = "Emecé",
                        Isbn = "978-8420633114",
                        AnioPublicacion = 1944,
                        Ejemplares = 2,
                        Disponible = true,
                        FechaRegistro = new DateTime(2026, 1, 15)
                    },
                    new Libro
                    {
                        Id = 3,
                        Titulo = "Cuentos de barro",
                        Autor = "Salarrué",
                        Editorial = "Dirección de Publicaciones",
                        Isbn = "978-9992300121",
                        AnioPublicacion = 1933,
                        Ejemplares = 1,
                        Disponible = false,
                        FechaRegistro = new DateTime(2026, 1, 15)
                    }
                );
            });
        }
    }
}
