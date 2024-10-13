using System.ComponentModel.DataAnnotations;

namespace Proyect_1.Models
{
    public class User
    {
        // modelo de usuario.
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{6,}$", ErrorMessage = "La contraseña debe tener al menos 6 caracteres, con la primera letra mayúscula y al menos un carácter especial.")]
        public string? Password { get; set; }
        public bool? Captcha { get; set; }
        public string? RecaptchaToken { get; set; }

      

        [Required(ErrorMessage = "La confirmación de la contraseña es requerida.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }
    }
}
