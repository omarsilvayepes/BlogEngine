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
            Post posts = await mongoCollection.FindAsync(new BsonDocument { { "_id", new ObjectId(comment.IdPost) }, { "status", "published" } }).Result.FirstOrDefaultAsync();

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
            return "Cannot add comment";
        }

        public async Task<Post> createOrUpdate(Post post)
        {
            post.Date = DateTime.Now;
            //post.isLocked = false;
            post.status = "created";
            if (post.Id != null)
            {
                var postDB = Builders<Post>.Filter.Eq(resultado => resultado.Id, post.Id);//update
                await mongoCollection.ReplaceOneAsync(postDB, post);
                return post;
            }
            await mongoCollection.InsertOneAsync(post);
            return post;
        }

        

        public async Task<List<Post>> getPublishedPosts()
        {
            List<Post> lista = await mongoCollection.FindAsync(new BsonDocument { { "status", "published" } }).Result.ToListAsync();
            return lista;
        }

        public async Task<List<Post>> getCreateAndPendingPosts()
        {
            List<Post> lista = await mongoCollection.FindAsync(new BsonDocument { { "status", "created" } } ).Result.ToListAsync();//pending or??
            return lista;
        }

        public async  Task<string> SubmitPost(Post post)
        {
            Post posts = await mongoCollection.FindAsync(new BsonDocument { { "_id", new ObjectId(post.Id) }, { "status", "created" } }).Result.FirstOrDefaultAsync();

            if (posts != null)
            {
                //post.isLocked = true;
                var postDB = Builders<Post>.Filter.Eq(result => result.Id, post.Id);
                var updateStatusPost = Builders<Post>.Update.Set("status", "pending");
                await mongoCollection.UpdateOneAsync(postDB, updateStatusPost);
                return "OK";
            }
            return "Cannot submit post";
        }

        
        public async Task<List<Post>> getPendingPosts()
        {
            List<Post> lista = await mongoCollection.FindAsync(new BsonDocument { { "status", "pending" } }).Result.ToListAsync();
            return lista;
        }

        public async  Task<string> ApprovePendingPost(Post post)
        {
            Post posts = await mongoCollection.FindAsync(new BsonDocument { { "_id", new ObjectId(post.Id) }, { "status", "pending" } }).Result.FirstOrDefaultAsync();

            if (posts != null)
            {
                var postDB = Builders<Post>.Filter.Eq(result => result.Id, post.Id);
                var updateStatusPost = Builders<Post>.Update.Set("status", "published");
                await mongoCollection.UpdateOneAsync(postDB, updateStatusPost);
                return "OK";
            }
            return "Cannot Approve post";
        }

        public async Task<string> RejectPendingPost(Post post)
        {
            Post posts = await mongoCollection.FindAsync(new BsonDocument { { "_id", new ObjectId(post.Id) }, { "status", "pending" } }).Result.FirstOrDefaultAsync();

            if (posts != null)
            {
                var postDB = Builders<Post>.Filter.Eq(result => result.Id, post.Id);
                var updateStatusPost = Builders<Post>.Update.Set("status", "created");
                await mongoCollection.UpdateOneAsync(postDB, updateStatusPost);
                return "OK";
            }
            return "Cannot Reject post";
        }
    }
}
