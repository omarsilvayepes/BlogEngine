using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogEngine.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserName { get; set; }
        public byte[] PassWordHash { get; set; }
        public byte[] PassWordSalt { get; set; }
    }
}
