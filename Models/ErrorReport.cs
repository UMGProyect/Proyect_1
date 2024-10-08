namespace Proyect_1.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ErrorReport
    {
        [Required]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres.")]
        public string? Description { get; set; }
        public string ErrorDetectado { get; internal set; }
    }

}
