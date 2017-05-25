using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SampleAADV2Bot.Helpers
{
    public class MeetingRoom
    {
        public string DisplayName { get; set; }
        public string LocationEmailAddress { get; set; }
    }

    public class UserInfo
    {
        public string DisplayName { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string JobTitle { get; set; }
        public string Mail { get; set; }

    }
    public class GraphHelper
    {
        
        private string _token;

        


        public GraphHelper(string token)
        {
            _token = token;
        }

        public async Task<UserInfo > GetUserInfo()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + this._token);

                var userresponse = await client.GetAsync("https://graph.microsoft.com/beta/me/");

                if (userresponse.StatusCode != System.Net.HttpStatusCode.OK )
                {
                    Trace.WriteLine("GetUserInfo , StatusCode={0}\n", userresponse.StatusCode.ToString());
                    return null;
                }
                dynamic userInfo = JObject.Parse(await userresponse.Content.ReadAsStringAsync());

                return new UserInfo()
                {
                    DisplayName = userInfo.displayName,
                    Firstname = userInfo.givenName,
                    Mail = userInfo.userPrincipalName,
                    JobTitle = userInfo.jobTitle
                };
            }
            catch (Exception)
            {
                throw;
            }           
        }

        public async Task<List<MeetingRoom>> GetMeetingRoomSuggestions()
        {
            try
            {

                List<MeetingRoom> suggestions = new List<MeetingRoom>();
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + this._token);

                var meetingresponse = await client.PostAsync("https://graph.microsoft.com/beta/me/findMeetingTimes", new StringContent(String.Empty));

                if (meetingresponse.StatusCode !=  System.Net.HttpStatusCode.OK )
                {
                    Trace.WriteLine("GetMeetingRoomSuggestions , StatusCode={0}", meetingresponse.StatusCode.ToString());
                }
                dynamic meetingTimes = JObject.Parse(await meetingresponse.Content.ReadAsStringAsync());

                foreach (var item in meetingTimes.meetingTimeSuggestions[0].locations)
                {
                    // Add only locations with an email address -> meeting rooms
                    if (!String.IsNullOrEmpty(item.locationEmailAddress.ToString()))
                        suggestions.Add(new MeetingRoom()
                        {
                            DisplayName = item.displayName,
                            LocationEmailAddress = item.locationEmailAddress
                        });
           
                }

                return suggestions;
            }
            catch (Exception)
            {

                throw;
            }
        
        
        }

    }
}