using Microsoft.AspNetCore.Mvc;
using fitate.Utils;
using System.Text.Json;
using fitate.Models;
using MongoDB.Driver;

namespace fitate.Controllers;

[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserUtils _userUtils;
    private readonly DatabaseUtils.DatabaseUtils _databaseUtils;
    public UserController(UserUtils userUtils, DatabaseUtils.DatabaseUtils databaseUtils)
    {
        _userUtils = userUtils;
        _databaseUtils = databaseUtils;
    }
    [HttpGet("getUser")]
    public async Task<IActionResult> GetUser()
    {
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
                return new JsonResult(user)
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
            Console.WriteLine($"Verification failed: {e.Message}");
            throw;
        }
    }
    [HttpPost("dish")]
    public async Task<IActionResult> CreateDish([FromBody] UserDish dish)
    {
        try
        {
            string token = Request.Headers.Authorization;
            var collection = _databaseUtils.GetUserCollection();

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
                var getUserFilter = Builders<UserModel>.Filter.Eq(u => u.UID, uid);
                UserModel user = await collection.Find(getUserFilter).FirstOrDefaultAsync();
                var newDish = new UserDish
                {
                    DishId = dish.DishId,
                    UserDishId = DatabaseUtils.DatabaseUtils.RandomString(16),
                    Portion = float.Parse(dish.Portion.ToString($"F{1}")),
                    MealTime = dish.MealTime,
                    Date = dish.Date
                };
                
                user.Dishes.Add(newDish);

                var addDish = Builders<UserModel>.Update.Set(u => u.Dishes, user.Dishes);
                var addDishQuery = await collection.UpdateOneAsync(getUserFilter, addDish);
                if (addDishQuery.ModifiedCount == 1)
                {
                    return new ObjectResult("Successfully added the dish")
                    {
                        StatusCode = 200
                    };
                }
                return new ObjectResult("Could not add the dish")
                {
                    StatusCode = 500
                };
            }
            return new ObjectResult("Unauthorized")
            {
                StatusCode = 403
            };
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"Verification failed: {e.Message}");
            throw;
        }
    }
    [HttpGet("dishes")]
    public async Task<IActionResult> GetDishes()
    {
        var collection = _databaseUtils.GetDishCollection();
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
                List<UserDishModel.UserDish> dishes = new List<UserDishModel.UserDish>();
                var user = await _userUtils.GetUserByUID(uid);
                foreach (var userDish in user.Dishes)
                {
                    var dish = await collection.Find(Builders<DishModel.Dish>.Filter.Eq(d => d.DishId, userDish.DishId)).FirstOrDefaultAsync();

                    if (dish != null)
                    {
                        float totalCalories = (dish.TotalCallories ?? 0) * userDish.Portion; // actually never is equal to 0
                        UserDishModel.UserDish formattedDish = new UserDishModel.UserDish
                        {
                            UserDishId = userDish.UserDishId,
                            Name = dish.Name,
                            Proteins = dish.Proteins * userDish.Portion,
                            Carbs = dish.Carbs * userDish.Portion,
                            Fats = dish.Fats * userDish.Portion,
                            TotalCallories = totalCalories,
                            Date = userDish.Date,
                            MealTime = userDish.MealTime
                        };
                        dishes.Add(formattedDish);
                    }
                }
                return new JsonResult(dishes)
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
            Console.WriteLine($"Verification failed: {e.Message}");
            throw;
        }
    }
    [HttpDelete("dish")]
    public async Task<IActionResult> DeleteDish([FromBody] UserDishModel.UserDish request)
    {
        try
        {
            string token = Request.Headers.Authorization;
            var collection = _databaseUtils.GetUserCollection();

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
                var getUserFilter = Builders<UserModel>.Filter.Eq(u => u.UID, uid);
                var update = Builders<UserModel>.Update.PullFilter(u => u.Dishes, Builders<UserDish>.Filter.Eq(d => d.UserDishId, request.UserDishId));
            
                var updateResult = await collection.UpdateOneAsync(getUserFilter, update);

                if (updateResult.ModifiedCount > 0)
                {
                    return new ObjectResult("Successfully deleted the dish")
                    {
                        StatusCode = 200
                    };
                }

                return new ObjectResult("Dish not found or could not be deleted")
                {
                    StatusCode = 404
                };
            }

            return new ObjectResult("Unauthorized")
            {
                StatusCode = 403
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Verification failed: {e.Message}");
            throw;
        }
    }


    [HttpPost("workout")]
    public async Task<IActionResult> CreateWorkout([FromBody] UserWorkout userWorkout)
    {
        try
        {
            string token = Request.Headers.Authorization;
            var collection = _databaseUtils.GetUserCollection();

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
                var getUserFilter = Builders<UserModel>.Filter.Eq(u => u.UID, uid);
                UserModel user = await collection.Find(getUserFilter).FirstOrDefaultAsync();
                var newWorkout = new UserWorkout
                {
                    WorkoutID = userWorkout.WorkoutID,
                    UserWorkoutID = DatabaseUtils.DatabaseUtils.RandomString(16),
                    Date = userWorkout.Date,
                    Distance = userWorkout.Distance != null ? userWorkout.Distance : 0,
                    Reps = userWorkout.Reps != null ? userWorkout.Reps : 0,
                    Weight = userWorkout.Weight != null ? userWorkout.Weight : 0
                };
                
                user.Workouts.Add(newWorkout);

                var addWorkout = Builders<UserModel>.Update.Set(u => u.Workouts, user.Workouts);
                var addWorkoutQuery = await collection.UpdateOneAsync(getUserFilter, addWorkout);
                if (addWorkoutQuery.ModifiedCount == 1)
                {
                    return new ObjectResult("Successfully added the workout")
                    {
                        StatusCode = 200
                    };
                }
                return new ObjectResult("Could not add the workout")
                {
                    StatusCode = 500
                };
            }
            return new ObjectResult("Unauthorized")
            {
                StatusCode = 403
            };
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"Verification failed: {e.Message}");
            throw;
        }
    }
    [HttpGet("workouts")]
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
                List<UserWorkoutModel.UserWorkout> workouts = new List<UserWorkoutModel.UserWorkout>();
                var user = await _userUtils.GetUserByUID(uid);
                foreach (var userWorkout in user.Workouts)
                {
                    var workout = await collection.Find(Builders<WorkoutModel.Workout>.Filter.Eq(w => w.WorkoutID, userWorkout.WorkoutID)).FirstOrDefaultAsync();

                    if (workout != null)
                    {
                        if (!workout.CaloriesBurnedPerKm.HasValue && workout.CaloriesBurnedPerRep.HasValue)
                        {
                            float? totalCalories = (workout.CaloriesBurnedPerRep ?? 0) * userWorkout.Reps;
                            UserWorkoutModel.UserWorkout formattedWorkout = new UserWorkoutModel.UserWorkout
                            {
                                Name = workout.Name,
                                UserWorkoutID = userWorkout.UserWorkoutID,
                                CaloriesBurned = totalCalories,
                                Reps = userWorkout.Reps,
                                Date = userWorkout.Date
                            };
                            workouts.Add(formattedWorkout);
                        }
                        else
                        {
                            float? totalCalories = (workout.CaloriesBurnedPerKm ?? 0) * userWorkout.Distance;
                            UserWorkoutModel.UserWorkout formattedWorkout = new UserWorkoutModel.UserWorkout
                            {
                                UserWorkoutID = userWorkout.UserWorkoutID,
                                Name = workout.Name,
                                CaloriesBurned = totalCalories,
                                Distance = userWorkout.Distance,
                                Date = userWorkout.Date
                            };
                            workouts.Add(formattedWorkout);
                        }
                    }
                }
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
            Console.WriteLine($"Verification failed: {e.Message}");
            throw;
        }
    }
    [HttpDelete("workout")]
    public async Task<IActionResult> DeleteWorkout([FromBody] UserWorkoutModel.UserWorkout request)
    {
        try
        {
            string token = Request.Headers.Authorization;
            var collection = _databaseUtils.GetUserCollection();

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
                var getUserFilter = Builders<UserModel>.Filter.Eq(u => u.UID, uid);
                var update = Builders<UserModel>.Update.PullFilter(u => u.Workouts, Builders<UserWorkout>.Filter.Eq(w => w.UserWorkoutID, request.UserWorkoutID));

                var updateResult = await collection.UpdateOneAsync(getUserFilter, update);

                if (updateResult.ModifiedCount > 0)
                {
                    return new ObjectResult("Successfully deleted the workout")
                    {
                        StatusCode = 200
                    };
                }

                return new ObjectResult("Workout not found or could not be deleted")
                {
                    StatusCode = 404
                };
            }

            return new ObjectResult("Unauthorized")
            {
                StatusCode = 403
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Verification failed: {e.Message}");
            throw;
        }
    }


    [HttpPost("info")]
    public async Task<IActionResult> SetInfo([FromBody] Info info)
    {
        try
        {
            string token = Request.Headers.Authorization;
            var collection = _databaseUtils.GetUserCollection();

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
                var getUserFilter = Builders<UserModel>.Filter.Eq(u => u.UID, uid);
                UserModel user = await collection.Find(getUserFilter).FirstOrDefaultAsync();

                var newInfo = new Info
                {
                    Height = info.Height,
                    Age = info.Age,
                    Gender = info.Gender,
                    Weight = info.Weight,
                    ActivityLevel = info.ActivityLevel
                };

                var setInfo = Builders<UserModel>.Update.Set(u => u.Info, newInfo);
                var setInfoQuery = await collection.UpdateOneAsync(getUserFilter, setInfo);

                if (setInfoQuery.ModifiedCount == 1)
                {
                    return new ObjectResult("Successfully set info")
                    {
                        StatusCode = 200
                    };
                }

                return new ObjectResult("Could not set info")
                {
                    StatusCode = 500
                };
            }

            return new ObjectResult("Unauthorized")
            {
                StatusCode = 403
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Verification failed: {e.Message}");
            throw;
        }
    }

}