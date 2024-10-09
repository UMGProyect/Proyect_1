using System.Collections.Generic;
using System.Threading.Tasks;
using Proyect_1.Models;
namespace Proyect_1.Services
{
    // Definición del servicio
    public class ReportService
    {
        // Simulación de datos que retornarán en un arreglo JSON
        public async Task<List<object>> ConsultarDatos()
        {
            // Simulamos una demora en la respuesta como si se tratara de una consulta real a la base de datos
            await Task.Delay(500);

            // Simulación de datos en formato JSON
            var datos = new List<object>
            {
                new { ID = "1", Nombre = "Pablo", Roles = "Administrador" },
                new { ID = "2", Nombre = "Dayana", Roles = "Usuario" },
                new { ID = "3", Nombre = "Gaby", Roles = "Moderador" },
                new { ID = "4", Nombre = "Mario", Roles = "Analista de pruebas"},
            };

            return datos; // Retornar los datos simulados
        }
    }
}
