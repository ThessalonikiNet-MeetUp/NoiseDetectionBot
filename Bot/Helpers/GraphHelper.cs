using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace NoiseDetectionBot.Helpers
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

        public async Task<Tuple<bool, UserInfo>> GetUserInfo()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + this._token);

                    var response = await client.GetAsync("https://graph.microsoft.com/beta/me/");
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Trace.TraceError($"GetUserInfo: StatusCode={response.StatusCode}");
                        return Tuple.Create<bool, UserInfo>(false, null);
                    }

                    dynamic responseBody = JObject.Parse(await response.Content.ReadAsStringAsync());

                    var userInfo = new UserInfo()
                    {
                        DisplayName = responseBody.displayName,
                        Firstname = responseBody.givenName,
                        Mail = responseBody.userPrincipalName,
                        JobTitle = responseBody.jobTitle,
                    };

                    return Tuple.Create(true, userInfo);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError($"GetUserInfo: Exception={e.Message}.");
                return Tuple.Create<bool, UserInfo>(false, null);
            }
        }

        public async Task<List<MeetingRoom>> GetMeetingRoomSuggestions()
        {
            var suggestions = new List<MeetingRoom>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {this._token}");

                    var meetingResponse = await client.PostAsync("https://graph.microsoft.com/beta/me/findMeetingTimes", new StringContent(String.Empty));

                    if (meetingResponse.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Trace.TraceError($"GetMeetingRoomSuggestions: StatusCode={meetingResponse.StatusCode}");
                        return suggestions;
                    }

                    dynamic meetingTimes = JObject.Parse(await meetingResponse.Content.ReadAsStringAsync());

                    foreach (var item in meetingTimes.meetingTimeSuggestions.First().locations)
                    {
                        // Add only locations with an email address -> meeting rooms
                        if (!String.IsNullOrEmpty(item.locationEmailAddress.ToString()))
                        {
                            suggestions.Add(new MeetingRoom()
                            {
                                DisplayName = item.displayName,
                                LocationEmailAddress = item.locationEmailAddress
                            });
                        }
                    }

                    return suggestions;
                }

            }
            catch (Exception e)
            {
                Trace.TraceError($"GetMeetingRoomSuggestions: Exception={e.Message}.");
                return suggestions;
            }
        }
    }
}