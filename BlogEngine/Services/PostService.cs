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
        public async Task<string> AddCommentById(Comment comment)
        {
            Post posts = await mongoCollection.FindAsync(new BsonDocument { { "_id", new ObjectId(comment.IdPost) }}).Result.FirstOrDefaultAsync();

            if (posts != null)
            {
                List<Comment> comments = posts.Comments;
                comments.Add(new Comment { 
                    IdPost=comment.IdPost,
                    Date = DateTime.Now,
                    comment=comment.comment,
                    Author =comment.Author
                });

                var postDB = Builders<Post>.Filter.Eq(result => result.Id, comment.IdPost);
                var updateCommentPost = Builders<Post>.Update.Set("Comments", comments);
                await mongoCollection.UpdateOneAsync(postDB, updateCommentPost);
                return "OK";
            }
            return "Post doesn´t exist";
        }

        public async Task<Post> createOrUpdate(Post post)
        {
                   
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

        public async Task<List<Post>> getPublishedPosts()
        {
            List<Post> lista = await mongoCollection.FindAsync(new BsonDocument { { "status", "published" } }).Result.ToListAsync();
            return lista;
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
