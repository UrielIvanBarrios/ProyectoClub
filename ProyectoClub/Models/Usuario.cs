using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProyectoClub.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Nombres { get; set; }
        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }
        [Required]
        [StringLength(250)]
        public string Direccion { get; set; }

        public bool Alta { get; set; } = true;  
    }
}
