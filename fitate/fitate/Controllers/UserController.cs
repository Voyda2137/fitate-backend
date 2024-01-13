using Microsoft.AspNetCore.Mvc;
using fitate.Utils;
using System.Text.Json;

namespace fitate.Controllers;

[Route("api/user/getUser")]
public class UserController : ControllerBase
{
    private readonly UserUtils _userUtils;
    public UserController(UserUtils userUtils)
    {
        _userUtils = userUtils;
    }
    [HttpGet]
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
}