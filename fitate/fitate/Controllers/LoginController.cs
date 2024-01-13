using Firebase.Auth;
using fitate.Models;
using fitate.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FirebaseAuthException = Firebase.Auth.FirebaseAuthException;
namespace fitate.Controllers;

[Route("api/user")]

public class LoginController: ControllerBase
{
    private FirebaseAuthProvider auth;

    private readonly DatabaseUtils.DatabaseUtils _databaseUtils;

    private UserUtils tokenVerifier;
    
    public LoginController(UserUtils userUtils)
    {
        DotNetEnv.Env.Load();
        tokenVerifier = userUtils;
        auth = new FirebaseAuthProvider(new FirebaseConfig(Environment.GetEnvironmentVariable("FIREBASE_KEY")));
        _databaseUtils = new DatabaseUtils.DatabaseUtils();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Registration([FromBody] LoginModel loginModel)
    {
        var collection = _databaseUtils.GetUserCollection();
        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
            var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
            
            string token = fbAuthLink.FirebaseToken;
            
            string uid = await tokenVerifier.VerifyUser(token);

            var newUser = new UserModel
            {
                UID = uid,
                Name = loginModel.Email
            };
            
            collection.InsertOne(newUser);
            
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
                token = token
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
}