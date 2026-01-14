namespace Locust.ViewModels
{
    public class CommentViewModel
    {
        public string username;
        public string text;
        public int likes;
        public int postId;
        public int id;

        public CommentViewModel(string commentUsername, string commentText, int commentLikes, int commentPostId)
        {
            username = commentUsername;
            text = commentText;
            likes = commentLikes;
            postId = commentPostId;
        }
    }
}
