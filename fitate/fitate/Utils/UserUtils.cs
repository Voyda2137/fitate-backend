using FirebaseAdmin.Auth;
using MongoDB.Driver;
using fitate.Models;

namespace fitate.Utils;

public class UserUtils
{
    private readonly IMongoCollection<UserModel> _userCollection;
    public UserUtils(DatabaseUtils.DatabaseUtils databaseUtils)
    {
        _userCollection = databaseUtils.GetUserCollection();
    }
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
}