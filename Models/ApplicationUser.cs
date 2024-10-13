using Microsoft.AspNetCore.Identity;

namespace Proyect_1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}
