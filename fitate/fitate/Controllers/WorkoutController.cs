using System.Text.Json;
using fitate.Models;
using fitate.Utils;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace fitate.Controllers;

[Route("api/workout")]

public class WorkoutController : ControllerBase
{
    private readonly WorkoutUtils _workoutUtils;
    private readonly DatabaseUtils.DatabaseUtils _databaseUtils;
    private readonly UserUtils _userUtils;

    public WorkoutController(WorkoutUtils workoutUtils, DatabaseUtils.DatabaseUtils databaseUtils, UserUtils userUtils)
    {
        _workoutUtils = workoutUtils;
        _databaseUtils = databaseUtils;
        _userUtils = userUtils;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateDish([FromBody] WorkoutModel.Workout workoutModel)
    {
        var collection = _databaseUtils.GetWorkoutCollection();
        try
        {
            string token = Request.Headers.Authorization;
            
            if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
            {
                return new ObjectResult("Unauthorized")
                {
                    StatusCode = 403
                };
            }
            
            token = token.Substring("Bearer ".Length).Trim();
            
            string uid = await _userUtils.VerifyUser(token);

            if (!string.IsNullOrEmpty(uid))
            {
                var user = await _userUtils.GetUserByUID(uid);

                if (workoutModel.CaloriesBurnedPerKm.HasValue)
                {
                    WorkoutModel.Workout newWorkout = new WorkoutModel.Workout
                    {
                        Name = workoutModel.Name,
                        CaloriesBurnedPerKm = workoutModel.CaloriesBurnedPerKm
                    };
                    collection.InsertOne(newWorkout);
                    return new ObjectResult("Successfully created the workout!")
                    {
                        StatusCode = 200
                    };
                }
                else if (workoutModel.CaloriesBurnedPerRep.HasValue)
                {
                    WorkoutModel.Workout newWorkout = new WorkoutModel.Workout
                    {
                        Name = workoutModel.Name,
                        CaloriesBurnedPerRep = workoutModel.CaloriesBurnedPerRep
                    };
                    collection.InsertOne(newWorkout);
                    return new ObjectResult("Successfully created the workout!")
                    {
                        StatusCode = 200
                    };
                }
            }
            return new ObjectResult("Unauthorized")
            {
                StatusCode = 403
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet("getWorkouts")]
    public async Task<IActionResult> GetWorkouts()
    {
        var collection = _databaseUtils.GetWorkoutCollection();
        try
        {
            string token = Request.Headers.Authorization;
            
            if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
            {
                return new ObjectResult("Unauthorized")
                {
                    StatusCode = 403
                };
            }
            
            token = token.Substring("Bearer ".Length).Trim();
            
            string uid = await _userUtils.VerifyUser(token);

            if (!string.IsNullOrEmpty(uid))
            {
                var workouts = await collection.Find(_ => true).ToListAsync();
                return new JsonResult(workouts)
                {
                    SerializerSettings = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }
                };
                
            }
            return new ObjectResult("Unauthorized")
            {
                StatusCode = 403
            };
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}