/* DatabaseServices.cs
 * Database helper for inserting, Retrieving, updating and deleting data in Firebase's Realtime database
 * 
 */

using Project_Portal.Models;
using MessagePack.Formatters;
using FireSharp;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using Newtonsoft.Json;
using System.Net;

namespace Project_Portal.Services
{
    public class DatabaseServices
    {
        //DB URL & secret
        private const string dbURL = "https://portal-project-14039-default-rtdb.asia-southeast1.firebasedatabase.app/";
        private const string auth = "BJm0Xt86MfbcKsarwCPzTvT2zfOcGw72OEW5XUzq";

        //Create DB connection with secret and DB url

        //private readonly FirebaseClient firebaseClient = new FirebaseClient(dbURL,
        //    new FirebaseOptions
        //    {
        //        AuthTokenAsyncFactory = () => Task.FromResult(auth)
        //    });

        private readonly static IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = auth,
            BasePath = dbURL
        };
        IFirebaseClient client = new FirebaseClient(config);

        public async void AddUser(RegistrationModel user)
        {
            await client.PushAsync("Users", user);

            //await firebaseClient
            //                .Child("Users")
            //                .PostAsync(user);
        }

        public async Task<SetResponse> AddPresentation(PresentationModel presentation)
        {

            PushResponse response = await client.PushAsync("Presentation/", presentation);
            presentation.Id = response.Result.name;
            SetResponse setResponse = await client.SetAsync("Presentation/" + presentation.Id, presentation);

            return setResponse;
        }

        public async Task<PresentationModel> GetPresentationById(string id)
        {
            FirebaseResponse response = client.Get("Presentation/" + id);
            PresentationModel data = JsonConvert.DeserializeObject<PresentationModel>(response.Body);
            return data;
        }

        public async Task<HttpStatusCode> EditPresentationById(string id, PresentationModel presentation)
        {
            FirebaseResponse response = await client.UpdateAsync("Presentation/" + id, presentation);
            return response.StatusCode;
        }

    }
}
