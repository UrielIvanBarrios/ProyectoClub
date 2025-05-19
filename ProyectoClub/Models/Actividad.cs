using System.ComponentModel.DataAnnotations;

namespace ProyectoClub.Models
{
    public class Actividad
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(250)]
        public string Descripcion { get; set; }

        public bool Habilitada { get; set; } = true;

        public List<SedeActividad> Sedes { get; set; } = new List<SedeActividad>();

    }
}
