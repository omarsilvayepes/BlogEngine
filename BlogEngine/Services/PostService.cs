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
            mongoCollection = database.GetCollection<Post>(settings.PostCollection);

            
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

        public async Task<Post> CreatePost(Post post)
        {
            if(post != null)
            {
                post.Date = DateTime.Now;
                post.status = "created";
                await mongoCollection.InsertOneAsync(post);
                return post;
            }
            return null;
        }
        public async Task<Post> UpdatePost(Post post)
        {
            Post postDb = await mongoCollection.FindAsync(new BsonDocument { { "_id", new ObjectId(post.Id) } }).Result.FirstOrDefaultAsync();

            if (postDb != null && !post.status.Equals("published") && !post.status.Equals("pending"))
            {
                post.Date = DateTime.Now;
                var postDB = Builders<Post>.Filter.Eq(resultado => resultado.Id, post.Id);
                await mongoCollection.ReplaceOneAsync(postDB, post);
                return post;
            }
            return null;
        }

        public async Task<List<Post>> getPublishedPosts()
        {
            List<Post> lista = await mongoCollection.FindAsync(new BsonDocument { { "status", "published" } }).Result.ToListAsync();
            return lista;
        }

        public async Task<List<Post>> getCreateAndPendingPosts()
        {
            var filter = Builders<Post>.Filter.Eq(resultado => resultado.status, "created") | Builders<Post>.Filter.Eq(resultado => resultado.status, "pending");
            List<Post> lista = await mongoCollection.FindAsync(filter).Result.ToListAsync();
            return lista;
        }

        public async  Task<string> SubmitPost(Post post)
        {
            Post posts = await mongoCollection.FindAsync(new BsonDocument { { "_id", new ObjectId(post.Id) }, { "status", "created" } }).Result.FirstOrDefaultAsync();

            if (posts != null)
            {
                post.Date = DateTime.Now;   
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
                post.Date = DateTime.Now;
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
                post.Date = DateTime.Now;
                var postDB = Builders<Post>.Filter.Eq(result => result.Id, post.Id);
                var updateStatusPost = Builders<Post>.Update.Set("status", "rejected");
                await mongoCollection.UpdateOneAsync(postDB, updateStatusPost);
                return "OK";
            }
            return "Cannot Reject post";
        }

        public async Task<string> AddCommentRejectPost(Comment comment)
        {
            Post postsDb = await mongoCollection.FindAsync(new BsonDocument { { "_id", new ObjectId(comment.IdPost) }, { "status", "rejected" } }).Result.FirstOrDefaultAsync();

            if (postsDb != null)
            {
                List<Comment> comments = postsDb.Comments;
                comments.Add(new Comment
                {
                    IdPost = comment.IdPost,
                    Date = DateTime.Now,
                    comment = comment.comment,
                    Author = comment.Author
                });

                var postDB = Builders<Post>.Filter.Eq(result => result.Id, comment.IdPost);
                var updateCommentPost = Builders<Post>.Update.Set("Comments", comments);
                await mongoCollection.UpdateOneAsync(postDB, updateCommentPost);
                return "OK";
            }
            return "Cannot add comment";
        }

        public async Task<List<List<Comment>>> getCommentsRejectPost()
        {
            List<Post> lista = await mongoCollection.FindAsync(new BsonDocument { { "status", "rejected" } }).Result.ToListAsync();
            List<List<Comment>> comments = new List<List<Comment>>();
            foreach (var post in lista)
            {
                comments.Add(post.Comments);
            }
            return comments;
        }
    }
}
