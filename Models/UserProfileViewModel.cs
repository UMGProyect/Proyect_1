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
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; }
        public int Likes { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }

    public class CommentViewModel
    {
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; internal set; }
    }
}

