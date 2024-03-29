﻿/* DatabaseServices.cs
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
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Project_Portal.Services
{
    public class DatabaseServices
    {
        //DB URL & secret
        private const string dbURL = "https://portal-project-14039-default-rtdb.asia-southeast1.firebasedatabase.app/";
        private const string auth = "BJm0Xt86MfbcKsarwCPzTvT2zfOcGw72OEW5XUzq";

        private readonly static IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = auth,
            BasePath = dbURL
        };
        IFirebaseClient client = new FirebaseClient(config);

        public async void AddUser(RegistrationModel user)
        {
            PushResponse response = await client.PushAsync("Users/", user);
            user.Id = response.Result.name;
            SetResponse setResponse = await client.SetAsync("Users/" + user.Id, user);
        }

        public async Task<SetResponse> AddPresentation(PresentationModel presentation)
        {

            PushResponse response = await client.PushAsync("Presentation/", presentation);
            presentation.Id = response.Result.name;
            SetResponse setResponse = await client.SetAsync("Presentation/" + presentation.Id, presentation);

            return setResponse;
        }

        public async Task<SetResponse> AddAttendee(AttendeeModel attendee)
        {
            PushResponse response = await client.PushAsync("Attendance/", attendee);
            attendee.Id = response.Result.name;
            SetResponse setResponse = await client.SetAsync("Attendance/" + attendee.Id, attendee);

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

        public async Task<List<RegistrationModel>> GetAllUser()
        {
            FirebaseResponse response = await client.GetAsync("Users");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<RegistrationModel>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<RegistrationModel>(((JProperty)item).Value.ToString()));
                }
            }

            return list;
        }

        public async Task<List<AttendeeModel>> GetAllAttendee()
        {
            FirebaseResponse response = await client.GetAsync("Attendance");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<AttendeeModel>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<AttendeeModel>(((JProperty)item).Value.ToString()));
                }
            }

            return list;
        }

        public async Task<char> GetUserTypeByEmail(string email)
        {
            /**
            FirebaseResponse response = await client.GetAsync("Users");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            if (data != null)
            {
                foreach (var item in data)
                {
                    foreach (var detail in item)
                    {
                        if (detail.Id == id)
                        {
                            return detail.User_Type;
                        }
                    }
                }
            }
            return null;
            **/
            FirebaseResponse response = client.Get("Users");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<RegistrationModel>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<RegistrationModel>(((JProperty)item).Value.ToString()));
                }
            }

            char userType = 'S';
            // checking for matching email
            // retrieve matching email user type
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].Email == email)
                {
                    userType = list[i].User_Type;
                    break;
                }
            }
            
            return userType;

            
        }

        public async Task<string> GetUserEmailById(string id)
        {
            FirebaseResponse response = await client.GetAsync("Users");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            if (data != null)
            {
                foreach (var item in data)
                {
                    foreach (var detail in item)
                    {
                        if (detail.Id == id)
                        {
                            return detail.Email;
                        }
                    }
                }
            }
            return null;
        }

        public async Task<List<PresentationModel>> GetAllUpcomingPresentation()
        {
            FirebaseResponse response = await client.GetAsync("Presentation");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            DateTime currentDateTime = DateTime.Now;
            var upcomingProjectList = new List<PresentationModel>();

            if (data != null)
            {
                foreach (var item in data)
                {
                    foreach (var detail in item)
                    {
                        string datetime = detail.date + " " + detail.time;

                        DateTime presentationDateTime = DateTime.ParseExact(datetime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

                        if (presentationDateTime >= currentDateTime)
                        {
                            upcomingProjectList.Add(JsonConvert.DeserializeObject<PresentationModel>(((JProperty)item).Value.ToString()));
                        }

                    }
                }
            }

            return upcomingProjectList;
        }

        public async Task<List<PresentationModel>> GetAllCompletedPresentation()
        {
            FirebaseResponse response = await client.GetAsync("Presentation");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            DateTime currentDateTime = DateTime.Now;
            var completedProjectList = new List<PresentationModel>();

            if (data != null)
            {
                foreach (var item in data)
                {
                    foreach (var detail in item)
                    {
                        string datetime = detail.date + " " + detail.time;
                        DateTime presentationDateTime = DateTime.ParseExact(datetime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        if (presentationDateTime < currentDateTime)
                        {
                            completedProjectList.Add(JsonConvert.DeserializeObject<PresentationModel>(((JProperty)item).Value.ToString()));
                        }
                    }
                }
            }

            return completedProjectList;
        }

        public async Task<SetResponse> SubmitReview(AttendeeModel Review)
        {
            SetResponse setResponse =  await client.SetAsync("Attendance/" + Review.Id, Review);
            return setResponse;
        }


    }
}
