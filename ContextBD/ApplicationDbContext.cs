using Proyect_1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace Proyect_1.ContextBD
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Publicacion> Publicaciones { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
    }
}
