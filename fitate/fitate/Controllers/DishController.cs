using fitate.Models;
using fitate.Utils;
using Microsoft.AspNetCore.Mvc;

namespace fitate.Controllers;

[Route("api/dish")]
public class DishController : ControllerBase
{
    private readonly DishUtils _dishUtils;
    private readonly DatabaseUtils.DatabaseUtils _databaseUtils;
    private readonly UserUtils _userUtils;

    public DishController(DishUtils dishUtils, DatabaseUtils.DatabaseUtils databaseUtils, UserUtils userUtils)
    {
        _dishUtils = dishUtils;
        _databaseUtils = databaseUtils;
        _userUtils = userUtils;

    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateDish([FromBody] DishModel.Dish dishModel)
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
                var user = await _userUtils.GetUserByUID(uid);

                float proteins = (float)Math.Round(dishModel.Proteins, 1);
                float fats = (float)Math.Round(dishModel.Fats, 1);
                float carbs = (float)Math.Round(dishModel.Carbs, 1);

                float callories = (float)Math.Round(proteins * 4 + fats * 9 + carbs * 4, 1); // calculate callories from the formula and round to 1 decimal point

                DishModel.Dish newDish = new DishModel.Dish
                {
                    DefaultPortion = 100, //grams
                    Proteins = proteins,
                    Fats = fats,
                    Carbs = carbs,
                    TotalCallories = callories,
                    Name = dishModel.Name,
                    AuthorId = user.Id
                };
                
                collection.InsertOne(newDish);

                return new ObjectResult("Successfully created the dish!")
                {
                    StatusCode = 200
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