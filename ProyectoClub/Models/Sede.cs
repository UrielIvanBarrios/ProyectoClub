using System.ComponentModel.DataAnnotations;

namespace ProyectoClub.Models
{
    public class Sede
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(250)]
        public string Direccion { get; set; }

        [Required]
        [Range(1, 300000, ErrorMessage = "La capacidad debe ser mayor a 0 y menor a 5000.")]
        public int capacidad { get; set; }

        public bool Habilitada { get; set; } = true;

        public List<Evento> Eventos { get; set; } = new List<Evento>();

        public List<SedeActividad> Actividades { get; set; } = new List<SedeActividad>();

        public List<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();

    }
}
