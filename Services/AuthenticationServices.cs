using Firebase.Auth;
//using Firebase.Auth.Providers;

namespace Project_Portal.Services
{
    public class AuthenticationServices
    {
        // Declare firebase authentication object
        // Firebase web API link
        private readonly FirebaseAuthProvider auth = new FirebaseAuthProvider(
                         new FirebaseConfig("AIzaSyC1ZXHDjcnOy5lC24mCQcyaKPRFih7KP5Q"));


        public async Task<string> GetCurrentUser(string token)
        {
            User user = await auth.GetUserAsync(token);
            return user.LocalId.ToString();
        }

        public async Task<string> GetUserEmailByToken(string token)
        {
            User user = await auth.GetUserAsync(token);
            return user.Email;
        }

    };







}
