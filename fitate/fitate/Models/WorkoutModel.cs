using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fitate.Models;

public class WorkoutModel
{
    public class Workout
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string WorkoutID { get; set; }
        public string Name { get; set; }
        public int? CaloriesBurnedPerRep { get; set; }
        public int? CaloriesBurnedPerKm { get; set; }
    }
}
