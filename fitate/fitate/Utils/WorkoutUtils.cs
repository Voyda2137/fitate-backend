using fitate.Models;
using MongoDB.Driver;

namespace fitate.Utils;

public class WorkoutUtils
{
    private readonly IMongoCollection<WorkoutModel.Workout> _workoutCollection;

    public WorkoutUtils(DatabaseUtils.DatabaseUtils databaseUtils)
    {
        _workoutCollection = databaseUtils.GetWorkoutCollection();
    }
}