using MongoDB.Driver;

namespace GeneticAIApp.Data
{
    public class MongoDb
    {
        string mongoConnection = "mongodb://127.0.0.1:27017";
        string mongoDbName = "school_program";
        public readonly MongoClient mongoDbClient;
        public IMongoDatabase mongoDb;


        public MongoDb()
        {
            mongoDbClient = new MongoClient(mongoConnection);
            mongoDb = mongoDbClient.GetDatabase(mongoDbName);
            
        }

        public IMongoDatabase GetMongoDatabase()
        {
            return mongoDbClient.GetDatabase(mongoDbName);
        }

    }
}
