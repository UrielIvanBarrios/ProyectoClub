using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ProyectoClub.Models
{
    public class SedeActividad
    {
        [Required]
        public int SedeId { get; set; }
        [ValidateNever]
        public Sede Sede { get; set; }
        [Required]
        public int ActividadId { get; set; }
        [ValidateNever]
        public Actividad Actividad { get; set; }

    }
}
