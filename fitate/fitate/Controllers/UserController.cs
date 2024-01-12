using Microsoft.AspNetCore.Mvc;
using fitate.Utils;
using Microsoft.AspNetCore.Http.Headers;

namespace fitate.Controllers;

[Route("api/getUser")]
public class UserController : ControllerBase
{
    private readonly UserUtils _userUtils = new UserUtils();
    
    [HttpGet]
    public async Task<IActionResult> GetUser([FromHeader(Name = "Authorization")] string authHeader)
    {
        try
        {
            string token = Request.Headers["Authorization"];
            Console.WriteLine($"Tokenisko: {token}");
            string uid = await _userUtils.VerifyUser(token);

            if (!string.IsNullOrEmpty(uid))
            { 
                var user = await _userUtils.GetUserByUID(uid);
                return new JsonResult(user);
                
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