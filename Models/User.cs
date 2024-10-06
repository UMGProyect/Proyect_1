namespace Proyect_1.Models
{
    public class User
    {
        // modelo de usuario.
        public int? Id { get; set; } 
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string RecaptchaToken { get; set; }

    }
}
