using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameServer.Models
{
    //Stored in "saves" 
    //one document per save slot per user.
    public class SaveGame
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id{get; set;}

        //which user owns save
        [BsonElement("userId")]
        public string userId {get; set;} = string.Empty;

        // slot1, 2, 3 & autoSave
        [BsonElement("slot")]
        public string Slot {get; set;} = string.Empty;

        //FUll game state serialized as json string
        //Stored as raw string so server doesnt get confused.
        [BsonElement("stateJson")]
        public string StateJson {get; set;} = string.Empty;

        //Display info stored separately so it can display list without deserializing full state.
        [BsonElement("day")]
        public int Day {get; set;} 

        [BsonElement("level")]
        public int Level {get; set;}

        [BsonElement("characterName")]
        public string CharacterName {get; set;} = string.Empty;
        [BsonElement("savedAt")]
        public DateTime SavedAt {get; set;} = DateTime.UtcNow;
    }
    //Sent from browsergame when saving
    public class SaveRequest
    {
        public string StateJson {get; set;} = string.Empty;
        public int Day {get; set;}
        public int Level {get; set;}
        public string CharacterName {get; set;} = string.Empty;
    }
    //sent back when listing slots
    public class SlotInfo
    {
        public string Slot {get; set;} =string.Empty;
        public int Day {get; set;}
        public int Level {get; set;}
        public string CharacterName {get; set;} =string.Empty;
        public string SavedAt {get; set;} = string.Empty;
        public bool HasData {get; set;}
    }
}