using fitate.Models;
using MongoDB.Driver;

namespace fitate.DatabaseUtils;

public class DatabaseUtils
{
    private readonly IMongoDatabase _mongoDatabase;

    public DatabaseUtils(string connectionString, string dbName)
    {
        var client = new MongoClient(connectionString);
        _mongoDatabase = client.GetDatabase(dbName);
    }

    public IMongoCollection<UserModel> GetUserCollection<UserModel>(string collectionName)
    {
        return _mongoDatabase.GetCollection<UserModel>(collectionName);
    }
}