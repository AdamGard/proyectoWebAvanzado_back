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
        public DbSet<Progreso> Progresos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles", t =>
                {
                    t.HasCheckConstraint("CK_Roles_Estado", "Estado IN ('A', 'I', 'N')");
                });
                entity.HasKey(e => e.RolId);
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.Estado).IsRequired().HasColumnType("char(1)").HasMaxLength(1).HasDefaultValue("A");
                entity.Property(e => e.FechaCreacion).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios", t =>
                {
                    t.HasCheckConstraint("CK_Usuarios_Estado", "Estado IN ('A', 'I', 'N')");
                });
                entity.HasKey(e => e.UsuarioId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(120);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(120);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Estado).IsRequired().HasColumnType("char(1)").HasMaxLength(1).HasDefaultValue("A");
                entity.Property(e => e.FechaRegistro).HasColumnType("date").HasDefaultValueSql("CURRENT_DATE");
                entity.Property(e => e.FechaActualizacion).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(e => e.RolId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Actividad>(entity =>
            {
                entity.ToTable("Actividades", t =>
                {
                    t.HasCheckConstraint("CK_Actividades_Estado", "Estado IN ('A', 'I', 'N')");
                    t.HasCheckConstraint("CK_Actividades_Nivel", "Nivel IN ('Inicial', 'Basico', 'Intermedio', 'Avanzado')");
                });
                entity.HasKey(e => e.ActividadId);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Tipo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nivel).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Estado).IsRequired().HasColumnType("char(1)").HasMaxLength(1).HasDefaultValue("A");
                entity.Property(e => e.Fecha).HasColumnType("date");
                entity.Property(e => e.FechaCreacion).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.FechaActualizacion).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Responsable)
                      .WithMany()
                      .HasForeignKey(e => e.ResponsableUsuarioId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Progreso>(entity =>
            {
                entity.ToTable("Progresos", t =>
                {
                    t.HasCheckConstraint("CK_Progresos_Avance", "AvancePorcentaje BETWEEN 0 AND 100");
                    t.HasCheckConstraint("CK_Progresos_Estado", "Estado IN ('A', 'I', 'N')");
                });
                entity.HasKey(e => e.ProgresoId);
                entity.HasIndex(e => new { e.UsuarioId, e.ActividadId }).IsUnique();
                entity.Property(e => e.AvancePorcentaje).HasPrecision(5, 2);
                entity.Property(e => e.Nivel).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Estado).IsRequired().HasColumnType("char(1)").HasMaxLength(1).HasDefaultValue("A");
                entity.Property(e => e.FechaRegistro).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.FechaActualizacion).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Actividad)
                      .WithMany()
                      .HasForeignKey(e => e.ActividadId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
