using Microsoft.AspNetCore.Mvc;
using Proyect_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Proyect_1.Controllers
{
    public class UserProfileController : Controller
    {
        public IActionResult Index(int userId)
        {
            // Simulando datos de usuario
            var user = new UserProfileViewModel
            {
                UserName = "Juan Perez",
                Bio = "Desarrollador de software apasionado por las buenas prácticas de código.",
                ProfilePictureUrl = "https://www.example.com/imagen.jpg",
                BannerImageUrl = "https://www.example.com/banner.jpg",
                Posts = GetMockPosts()
            };

            return View(user);
        }

        // Simulación de publicaciones
        private List<PostViewModel> GetMockPosts()
        {
            return new List<PostViewModel>
            {
                new PostViewModel
                {
                    Title = "Mi primer post",
                    Content = "Este es el contenido de mi primer post.",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    ImageUrl = "https://www.example.com/post1.jpg",
                    Likes = 10,
                    Comments = new List<CommentViewModel>
                    {
                        new CommentViewModel { UserName = "Usuario1", Content = "Gran post!" },
                        new CommentViewModel { UserName = "Usuario2", Content = "Me gusta mucho lo que has escrito." }
                    }
                },
                new PostViewModel
                {
                    Title = "Post sobre C#",
                    Content = "Hoy aprendí sobre los delegados en C# y son realmente poderosos.",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    ImageUrl = "",
                    Likes = 25,
                    Comments = new List<CommentViewModel>
                    {
                        new CommentViewModel { UserName = "Usuario3", Content = "Interesante, ¿podrías compartir más ejemplos?" }
                    }
                }
            };
        }

        public IActionResult EditProfile(int userId)
        {
            // Simulando el perfil del usuario
            var user = new User
            {
                Id = userId,
                Name = "Juan Perez",
                Password = "12345",
                Captcha = true,
                RecaptchaToken = "token"
            };

            return View(user); // Aquí podrías tener un formulario para editar el perfil.
        }
    }
}
