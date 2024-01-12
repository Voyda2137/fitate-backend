using Firebase.Auth;
using FirebaseAdmin.Auth;
using fitate.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FirebaseAuthException = Firebase.Auth.FirebaseAuthException;

namespace fitate.Controllers;

[Route("api/user")]

public class LoginController: ControllerBase
{
    private FirebaseAuthProvider auth;

    public LoginController()
    {
        DotNetEnv.Env.Load();
        auth = new FirebaseAuthProvider(new FirebaseConfig(Environment.GetEnvironmentVariable("FIREBASE_KEY")));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Registration([FromBody] LoginModel loginModel)
    {
        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
            var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
            string token = fbAuthLink.FirebaseToken;
            var res = new
            {
                token = token, 
            };
            return new JsonResult(res);
        }
        catch (FirebaseAuthException e)
        {
            var firebaseEx = JsonConvert.DeserializeObject<FirebaseLoginAuth.FirebaseError>(e.ResponseData);
            var error = new
            {
                error = firebaseEx.error.message
            };
            return new JsonResult(error);
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        try
        {
            var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                
            string token = fbAuthLink.FirebaseToken;
            
            
            var res = new
            {
                token = token,
            };
            return new JsonResult(res);
        }
        catch (FirebaseAuthException e)
        {
            var firebaseEx = JsonConvert.DeserializeObject<FirebaseLoginAuth.FirebaseError>(e.ResponseData);
            var error = new
            {
                error = firebaseEx.error.message
            };
            return new JsonResult(error);
        }
    }

    [HttpPost("createRecord")]
    public async Task<IActionResult> CreateRecord([FromBody] CreateRecord createRecord)
    {
        try
        {

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}