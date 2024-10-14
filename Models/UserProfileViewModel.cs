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
        public int? PostId { get; set; } // necesito para referenciar en la Base de Datos
        public int UserId { get; set; } // Relación con el usuario
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; }
        public int Likes { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }

    public class CommentViewModel
    {
        public int CommentId { get; set; } // ID del comentario
        public int PostId { get; set; } // ID de la publicación a la que pertenece el comentario
        public int UserId { get; set; } // ID del usuario que hizo el comentario
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; internal set; }
    }
}

