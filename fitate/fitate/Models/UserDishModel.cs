namespace fitate.Models;

public class UserDishModel
{
    public class UserDish
    {
        public string Name { get; set; }
        public float Proteins { get; set; }
        public float Fats { get; set; }
        public float Carbs { get; set; }
        public float TotalCallories { get; set; }
        public MealTime MealTime { get; set; }
        public long Date { get; set; }
    }
}