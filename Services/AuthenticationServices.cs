using Firebase.Auth;
//using Firebase.Auth.Providers;

namespace Project_Portal.Services
{
    public class AuthenticationServices
    {
        //static FirebaseAuthConfig config = new FirebaseAuthConfig
        //{
        //    ApiKey = "AIzaSyC1ZXHDjcnOy5lC24mCQcyaKPRFih7KP5Q",
        //    AuthDomain = "portal-project-14039.firebaseapp.com",
        //    Providers = new FirebaseAuthProvider[]
        //    {
        //            //new GoogleProvider(),
        //            //new FacebookProvider(),
        //            //new TwitterProvider(),
        //            //new GithubProvider(),
        //            //new MicrosoftProvider(),
        //            new EmailProvider()
        //    }
        //}

        //FirebaseAuthClient client = new FirebaseAuthClient(config);


        // Declare firebase authentication object
        // Firebase web API link
        private readonly FirebaseAuthProvider auth = new FirebaseAuthProvider(
                         new FirebaseConfig("AIzaSyC1ZXHDjcnOy5lC24mCQcyaKPRFih7KP5Q"));


        public async Task<string> GetCurrentUser(string token)
        {
            User user = await auth.GetUserAsync(token);
            return user.LocalId.ToString();
        }

    };







}
