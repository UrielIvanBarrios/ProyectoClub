using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;


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
        
        [ValidateNever]
        public Sede Sede { get; set; }

        [ValidateNever]
        public string UsuarioId { get; set; }

        [ValidateNever]
        public Usuario Usuario { get; set; }

    }
}
