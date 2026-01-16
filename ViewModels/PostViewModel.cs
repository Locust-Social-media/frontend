namespace Locust.NewFolder
{
    public class PostViewModel
    {
        public int PostID { get; set; }
        public string Title { get; set; } = "";
        public string BodyText { get; set; } = "";
        public int Likes { get; set; }

        // later voor comments count
        public int Comments { get; set; }
    }
}
