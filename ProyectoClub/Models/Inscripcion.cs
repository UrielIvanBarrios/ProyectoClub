using System.ComponentModel.DataAnnotations;
using System;

namespace ProyectoClub.Models
{
    public class Inscripcion
    {
        public int Id { get; set; }
        [Required]
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int? ActividadId { get; set; }
        public Actividad Actividad { get; set; }    
        public int? EventoId { get; set; }
        public Evento Evento { get; set; }
        [Required]
        public int SedeId { get; set; }
        public Sede Sede { get; set; }

        [Required]
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;
    }
}
