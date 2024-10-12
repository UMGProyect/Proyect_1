namespace Proyect_1.Models
{
    public class DatosModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } // Ejemplo: "Actividad", "Moderación", "Engagement"
        public string Autor { get; set; } // Quién generó el reporte
        public string Estado { get; set; } // Ejemplo: "Pendiente", "Completado"
        public string Detalles { get; set; } // Información adicional o enlaces a datos específicos
    }
}