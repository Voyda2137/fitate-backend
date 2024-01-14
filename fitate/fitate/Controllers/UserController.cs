﻿using Microsoft.AspNetCore.Mvc;
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
                    Portion = float.Parse(dish.Portion.ToString($"F{1}")),
                    MealTime = dish.MealTime,
                    Day = dish.Day
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
                            Name = dish.Name,
                            Proteins = dish.Proteins * userDish.Portion,
                            Carbs = dish.Carbs * userDish.Portion,
                            Fats = dish.Fats * userDish.Portion,
                            TotalCallories = totalCalories,
                            Day = userDish.Day,
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
}