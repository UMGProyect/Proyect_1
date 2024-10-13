using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Proyect_1.Models
{
    public class Comentario
    {
        public int Id { get; set; }                // ID del comentario
        public string UsuarioId { get; set; }      // ID del usuario que hizo el comentario
        public int PublicacionId { get; set; }     // ID de la publicación asociada
        [Required]
        public string Contenido { get; set; }      // Contenido del comentario
        public DateTime Fecha { get; set; }        // Fecha en que se hizo el comentario

        public virtual ApplicationUser Usuario { get; set; } // Relación con el usuario
        public virtual Publicacion Publicacion { get; set; } // Relación con la publicación
    }
}
