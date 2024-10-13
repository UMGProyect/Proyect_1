namespace Proyect_1.Models
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; }
        public string BannerImageUrl { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
        public List<PostViewModel> Posts { get; set; }
    }

    public class PostViewModel
    {
        public string Title { get; set; }                // Título de la publicación
        public string Content { get; set; }              // Contenido de la publicación
        public DateTime CreatedAt { get; set; }          // Fecha de creación de la publicación
        public string ImageUrl { get; set; }             // URL de la imagen adjunta a la publicación (si existe)
        public int Likes { get; set; }                   // Número de "Me gusta"
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();  // Lista de comentarios de la publicación
        public int PostId { get; set; }                  // ID de la publicación
        public string UserName { get; set; }             // Nombre del usuario que realizó la publicación
        public string UserProfileImage { get; set; }     
    }


    public class CommentViewModel
    {
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; internal set; }
    }
}

