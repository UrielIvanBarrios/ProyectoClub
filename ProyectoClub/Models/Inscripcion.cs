using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoClub.Models
{
    public class Inscripcion
    {
        public int Id { get; set; }
        [ValidateNever]
        public string UsuarioId { get; set; }

        [ValidateNever]
        public Usuario Usuario { get; set; }
        public int? ActividadId { get; set; }

        [ValidateNever]
        public Actividad Actividad { get; set; }    
        public int? EventoId { get; set; }

        [ValidateNever]
        public Evento Evento { get; set; }

        [ValidateNever]
        public int SedeId { get; set; }

        [ValidateNever]
        public Sede Sede { get; set; }

        [Required]
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;
    }
}
