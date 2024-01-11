using Firebase.Auth;

namespace fitate.Controllers;

public class LoginController
{
    private FirebaseAuthProvider auth;

    public LoginController()
    {
        auth = new FirebaseAuthProvider(new FirebaseConfig(""))
    }
}