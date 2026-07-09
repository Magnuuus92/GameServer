using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameServer.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id {get; set;}

        [BsonElement("username")]
        public string Username {get; set;} = string.Empty;

        //Hash - no plaintxt pw stored
[BsonElement("passwordHash")]
public string passwordHash {get; set;} = string.Empty;
//Timestamp
[BsonElement("createdAt")]
public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    }


}