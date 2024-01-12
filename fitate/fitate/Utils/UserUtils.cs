using FirebaseAdmin.Auth;
using MongoDB.Driver;
using fitate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace fitate.Utils;

public class UserUtils
{
    private readonly IMongoCollection<UserModel> _userCollection;
    
    public async Task<string> VerifyUser(string token)
    {
        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            
            return decodedToken.Uid;

        }
        catch(FirebaseAuthException e)
        {
            Console.WriteLine($"Verification failed: {e.Message}");
            throw;
        }
    }
    public async Task<UserModel> GetUserByUID(string uid)
    {
        var filter = Builders<UserModel>.Filter.Eq(u => u.UID, uid);
        return await _userCollection.Find(filter).FirstOrDefaultAsync() ?? throw new Exception("User not found");
    }

    public string GetAuthHeader(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValue))
        {
            string authHeader = authHeaderValue;
            return authHeader;
        }

        return null;
    }
}