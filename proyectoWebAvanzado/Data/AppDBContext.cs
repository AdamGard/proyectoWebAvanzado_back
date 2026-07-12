using Microsoft.EntityFrameworkCore;
using proyectoWebAvanzado.Data.Entities;

namespace proyectoWebAvanzado.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext (DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Actividad> Actividades { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles", "dbo");
                entity.HasKey(e => e.RolId);
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.Estado).HasColumnType("char(1)").HasMaxLength(1);
            });
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios", "dbo");
                entity.HasKey(e => e.UsuarioId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Estado).HasColumnType("char(1)").HasMaxLength(1);

                entity.HasQueryFilter(e => e.Estado == "A");

                entity.HasOne(e => e.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(e => e.RolId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Actividad>(entity =>
            {
                entity.ToTable("Actividades", "dbo", t =>
                {
                    t.HasCheckConstraint("CK_Actividades_Estado", "Estado IN ('A', 'I', 'N')");
                    t.HasCheckConstraint("CK_Actividades_Nivel", "Nivel IN ('Inicial', 'Basico', 'Intermedio', 'Avanzado')");
                });
                entity.HasKey(e => e.ActividadId);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Tipo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nivel).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Estado).HasMaxLength(1);
                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.HasOne(e => e.Responsable)
                      .WithMany()
                      .HasForeignKey(e => e.ResponsableUsuarioId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
