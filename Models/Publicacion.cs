using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Proyect_1.ContextBD;

namespace Proyect_1.Models
{
    public class Publicacion
    {
        public int Id { get; set; }                // ID de la publicación
        public string? UsuarioId { get; set; }      // ID del usuario que hizo la publicación
        [Required]
        public string? Contenido { get; set; }      // Contenido de la publicación
        public DateTime Fecha { get; set; }        // Fecha en que se hizo la publicación

        public virtual ApplicationUser? Usuario { get; set; } // Relación con el usuario
        public virtual ICollection<Comentario>? Comentarios { get; set; } // Relación con los comentarios
    }
}
