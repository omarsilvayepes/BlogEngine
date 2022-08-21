using BlogEngine.Models;

namespace BlogEngine.Repositories
{
    public interface IPostRepository
    {
        Task<List<Post>> getPublishedPosts();
        Task<string> AddCommentById(Comment comment);
        Task<Post> CreatePost(Post post);//status: created- unlocked
        Task<Post> UpdatePost(Post post);//status: created or rejected- unlocked
        Task<List<Post>> getCreateAndPendingPosts();
        Task<string> SubmitPost(Post post);//status: pending-approval- locked for update
        Task<List<Post>> getPendingPosts();
        Task<string> ApprovePendingPost(Post post);// status: published:locked
        Task<string> RejectPendingPost(Post post);//status: created-unlocked
        Task<string> AddCommentRejectPost(Comment comment);
        Task<List<List<Comment>>> getCommentsRejectPost();
    }
}
