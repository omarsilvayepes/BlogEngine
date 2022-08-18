using BlogEngine.Models;
using BlogEngine.Repositories;
using BlogEngine.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlogEngine.Services
{
    public class PostService : IPostRepository
    {
        private readonly IMongoCollection<Post> mongoCollection;
        

        public PostService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            mongoCollection = database.GetCollection<Post>(settings.CollectionName);

            
        }
        public Task AddCommentById(string id, string comment)
        {
            throw new NotImplementedException();
        }

        public async Task<Post> createOrUpdate(Post post)
        {
            
            //Post foundPost = await mongoCollection.FindAsync(new BsonDocument { { "Id", post.Id } })
            //    .Result.FirstOrDefaultAsync();
        
            if (post.Id != null)
            {
                var postDB = Builders<Post>.Filter.Eq(resultado => resultado.Id, post.Id);//actualizacion
                await mongoCollection.ReplaceOneAsync(postDB, post);
                return post;
            }
            post.isLocked = false;
            post.status = "created";
            await mongoCollection.InsertOneAsync(post);
            return post;
        }

        public Task dealPendingPost(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<List<Post>> getPendingPosts(string status)
        {
            throw new NotImplementedException();
        }

        public Task<List<Post>> getPosts()
        {
            throw new NotImplementedException();
        }

        public Task<Post> getPostsByUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<Post> SubmitPost(Post post)
        {
            throw new NotImplementedException();
        }
    }
}
