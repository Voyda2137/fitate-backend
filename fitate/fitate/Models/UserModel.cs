using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fitate.Models;

public enum MealTime
{
    Breakfast = 0,
    Dinner = 1,
    Supper = 2
}

public enum ActivityLevelTypes {
    Sedentary = 0,
    LightActivity = 1,
    ModerateActivity = 2,
    VeryActive = 3
}

public class UserDish
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string DishId { get; set; }

    public string UserDishId { get; set; }
    public float Portion { get; set; }
    public MealTime MealTime { get; set; }
    public long Date { get; set; }
}

public class UserWorkout
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string WorkoutID { get; set; }
    public string UserWorkoutID { get; set; }
    public int? Reps { get; set; }
    public int? Weight { get; set; }
    public float? Distance { get; set; }
    public long Date { get; set; }
}

public class Info
{
    public int Height { get; set; }
    public bool Gender { get; set; }
    public int Age { get; set; }
    public int Weight { get; set; }
    public ActivityLevelTypes ActivityLevel { get; set; }
}

public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string UID { get; set; }
    public string Name { get; set; }
    
    public List<UserDish> Dishes { get; set; } = new List<UserDish>();
    public List<UserWorkout> Workouts { get; set; } = new List<UserWorkout>();
    public Info Info { get; set; } = new Info();
}
