using System.ComponentModel.DataAnnotations;
using System;


namespace ProyectoClub.Models
{
    public class Evento
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(250)]
        public string Descripcion { get; set; }
        [Required]
        public DateTime FechaInicio { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }
        [Required]
        public int SedeId { get; set; }
        public Sede Sede { get; set; }

    }
}
