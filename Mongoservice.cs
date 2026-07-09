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
            
        }
    }
}