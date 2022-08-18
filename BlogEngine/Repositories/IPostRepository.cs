using BlogEngine.Models;

namespace BlogEngine.Repositories
{
    public interface IPostRepository
    {
        Task<List<Post>> getPosts();
        Task AddCommentById(string id,string comment);
        Task<Post> createOrUpdate(Post post);//status created unlocked
        Task<Post> getPostsByUser(User user);

        Task<Post> SubmitPost(Post post);//status pending-approval locked for update

        Task<List<Post>> getPendingPosts(string status);
        Task dealPendingPost(Post post);//approve-> status publish- or reject status reject-unlocked
        
    }
}
