using MongoDB.Driver;
using GameServer.Models;

namespace GameServer.Services
{
    //registered as a singleton - one shared connection with atlas.
    public class MongoService
    {
        private readonly IMongoDatabase _db;
        public MongoService(IConfiguration config)
        {
            var connectionString = config["MongoDB:ConnectionString"]
            ??throw new InvalidOperationException("MongoDB connection string not configured.");

            var databaseName = config["MongoDB:DatabaseName"] ?? "gameproject";

            var client = new MongoClient(connectionString);
            _db = client.GetDatabase(databaseName);

            EnsureIndexes();
        }
        //Collection Accessors
        public IMongoCollection<User> Users => _db.GetCollection<User>("users");
        public IMongoCollection<SaveGame> SaveGames => _db.GetCollection<SaveGame>("saves");

        private void EnsureIndexes()
        {
            //unique index usernames
            var usernameIndex = Builders<Users>.indexKeys.Ascending(u => u.Username);
            Users.Indexes.CreateOne(new CreateIndexModel<User>(
                usernameIndex,
                new CreateIndexOptions { Unique = true}
            ));
            // compound index on userId + slot & ensure one doc per user per slot
            var saveIndex = Builder<SaveGames>.indexKeys
            .Ascending(s => s.UserId)
            .Ascending(s => s.Slot);
            SaveGames.Indexes.CreateOne(new CreateIndexModel<SaveGame>(
                saveIndex,
                new CreateIndexOptions { Unique = true}
            ));
        }
    }
}