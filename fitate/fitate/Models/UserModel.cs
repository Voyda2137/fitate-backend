using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fitate.Models;

public enum MealTime
{
    Breakfast,
    Dinner,
    Supper
}

public class Dish
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string DishId { get; set; }
    public int Portion { get; set; }
    public MealTime MealTime { get; set; }
    public long Day { get; set; }
}

public class Workout
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string WorkoutId { get; set; }
    public int? Reps { get; set; }
    public double? Weight { get; set; }
    public double? Distance { get; set; }
    public long Day { get; set; }
}

public class Goal
{
    public double StartingWeight { get; set; }
    public double DesiredWeight { get; set; }
    public long StartingWeek { get; set; }
    public long DesiredWeek { get; set; }
}

public class UserModel
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string UID { get; set; }
    public List<Dish> Dishes { get; set; } = new List<Dish>();
    public List<Workout> Workouts { get; set; } = new List<Workout>();
    public Goal Goal { get; set; } = new Goal();
}
