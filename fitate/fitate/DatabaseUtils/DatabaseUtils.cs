using fitate.Models;
using MongoDB.Driver;

namespace fitate.DatabaseUtils;

public class DatabaseUtils
{
    private readonly IMongoDatabase _mongoDatabase;

    public DatabaseUtils()
    {
        DotNetEnv.Env.Load();
        var client = new MongoClient(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"));
        _mongoDatabase = client.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME"));
    }

    public IMongoCollection<UserModel> GetUserCollection()
    {
        return _mongoDatabase.GetCollection<UserModel>("users");
    }
    
    public IMongoCollection<DishModel.Dish> GetDishCollection()
    {
        return _mongoDatabase.GetCollection<DishModel.Dish>("dishes");
    }
    public IMongoCollection<WorkoutModel.Workout> GetWorkoutCollection()
    {
        return _mongoDatabase.GetCollection<WorkoutModel.Workout>("workouts");
    }
}