namespace ProyectoClub.Models
{
    public class SedeActividad
    {
        public int SedeId { get; set; }
        public Sede Sede { get; set; }
        public int ActividadId { get; set; }
        public Actividad Actividad { get; set; }

    }
}
