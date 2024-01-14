using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fitate.Models;

public enum MealTime
{
    Breakfast = 1,
    Dinner = 2,
    Supper = 3
}

public class UserDish
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string DishId { get; set; }
    public float Portion { get; set; }
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
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string UID { get; set; }
    public string Name { get; set; }
    public List<UserDish> Dishes { get; set; } = new List<UserDish>();
    public List<Workout> Workouts { get; set; } = new List<Workout>();
    public Goal Goal { get; set; } = new Goal();
}
