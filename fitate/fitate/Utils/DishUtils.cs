using fitate.Models;
using MongoDB.Driver;

namespace fitate.Utils;

public class DishUtils
{
    private readonly IMongoCollection<DishModel.Dish> _dishCollection;

    public DishUtils(DatabaseUtils.DatabaseUtils databaseUtils)
    {
        _dishCollection = databaseUtils.GetDishCollection();
    }

}