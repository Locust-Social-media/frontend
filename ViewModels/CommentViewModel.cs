namespace Locust.ViewModels
{
    public class CommentViewModel
    {
        public string username;
        public string text;
        public int likes;

        public CommentViewModel(string commentUsername, string commentText, int commentLikes)
        {
            username = commentUsername;
            text = commentText;
            likes = commentLikes;
        }
    }
}
