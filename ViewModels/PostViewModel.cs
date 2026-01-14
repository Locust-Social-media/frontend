namespace Locust.NewFolder
{
    public class PostViewModel
    {
        public string title;
        public string bodyText;
        public int likes;
        public int comments;
        public int postID;
        public int userID;

        public PostViewModel(string postTitle, string postText, int postLikes, int postComments, int postId)
        {
            title = postTitle;
            bodyText = postText;
            likes = postLikes;
            comments = postComments;
            postID = postId;
        }
    }
}
