using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fitate.Models;

public class DishModel
{
    public class Dish
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DishId { get; set; }
        public string Name { get; set; }
        public int DefaultPortion { get; set; }
        public float Proteins { get; set; }
        public float Fats { get; set; }
        public float Carbs { get; set; }
        public float? TotalCallories { get; set; }
        public string Author { get; set; }
    }
}