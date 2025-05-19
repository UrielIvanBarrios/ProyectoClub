using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoClub.Models;
    
namespace ProyectoClub.Data
{
    public class ProyectoClubDbContext : IdentityDbContext<Usuario>
    {
        public ProyectoClubDbContext(DbContextOptions<ProyectoClubDbContext> options)
            : base(options)
        {

        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Sede> Sedes { get; set; }
        public DbSet<Actividad> Actividades { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<SedeActividad> SedesActividad { get; set; }
        public DbSet<Evento> Eventos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SedeActividad>()
                .HasKey(sa => new { sa.SedeId, sa.ActividadId });

            modelBuilder.Entity<SedeActividad>()
                .HasOne(sa => sa.Sede)
                .WithMany(s => s.Actividades)
                .HasForeignKey(sa => sa.SedeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SedeActividad>()
                .HasOne(sa => sa.Actividad)
                .WithMany(a => a.Sedes)
                .HasForeignKey(sa => sa.ActividadId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inscripcion>()
                .HasOne( i => i.Usuario)
                .WithMany()
                .HasForeignKey(i => i.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Sede)
                .WithMany(s => s.Inscripciones)
                .HasForeignKey(i => i.SedeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inscripcion>()
                 .HasOne(i => i.Actividad)
                 .WithMany()  
                 .HasForeignKey(i => i.ActividadId)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Evento)
                .WithMany()  // No es necesario tener un List de inscripciones en Evento
                .HasForeignKey(i => i.EventoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sede>()
                .HasMany( s => s.Eventos)
                .WithOne( e => e.Sede)
                .HasForeignKey(e => e.SedeId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
