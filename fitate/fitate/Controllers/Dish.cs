using Microsoft.AspNetCore.Mvc;

namespace fitate.Controllers;

[Route("api/dish")]
public class Dish : Controller
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return "Hello";
    }
}