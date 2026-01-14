namespace Locust.NewFolder
{
    public class PostViewModel
    {
        public string title;
        public string text;
        public int likes;
        public int comments;
        public int id;

        public PostViewModel(string postTitle, string postText, int postLikes, int postComments, int postId)
        {
            title = postTitle;
            text = postText;
            likes = postLikes;
            comments = postComments;
            id = postId;
        }
    }
}
