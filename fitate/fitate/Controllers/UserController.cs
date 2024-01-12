// using Firebase.Auth;
// using Microsoft.AspNetCore.Mvc;
//
// namespace fitate.Controllers;
//
// [Route("api/getUser")]
// public class UserController
// {
//     private FirebaseAuthProvider auth;
//
//     public UserController()
//     {
//         DotNetEnv.Env.Load();
//         auth = new FirebaseAuthProvider(new FirebaseConfig(Environment.GetEnvironmentVariable("FIREBASE_KEY")));
//     }
//
//     [HttpGet]
//     public async Task<IActionResult> GetUser([FromHeader(Name = "Authorization")] string authorizationHeader)
//     {
//         
//     }
// }