namespace fitate.Models;

public class UserWorkoutModel
{
    public class UserWorkout
    {
        public string WorkoutID { get; set; }
        public string Name { get; set; }
        public float? Distance { get; set; }
        public int? Reps { get; set; }
        public float? CaloriesBurned { get; set; }
        public long Date { get; set; }
    }
}