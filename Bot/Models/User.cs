using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SampleAADV2Bot.Models
{
    [DataContract]
    public class User
    {
        public static string BaseAddress = ConfigurationManager.AppSettings["Endpoint.BaseAddress"];
        public static string FunctionKey = ConfigurationManager.AppSettings["FunctionKey.SaveBotUser"];

        [DataMember]
        public string Email { get; internal set;  }

        [DataMember]
        public string DisplayName { get; internal set; }

        [DataMember]
        public string BotUserName { get; internal set; }

        [DataMember]
        public string BotUserId { get; internal set; }

        [DataMember]
        public string BotId { get; internal set; }

        [DataMember]
        public string BotName { get; internal set; }

        [DataMember]
        public string ConversationId { get; internal set; }

        [DataMember]
        public string ChannelId { get; internal set; }

        [DataMember]
        public string ServiceUrl { get; internal set; }

        [DataMember]
        public string Token { get; internal set; }

        [IgnoreDataMember]
        public string DeviceId { get; internal set; }

        public User(
            string email,
            string displayName,
            string botUserName, 
            string botUserId, 
            string botId, 
            string botName, 
            string serviceUrl,
            string token,
            string conversationId, 
            string channelId)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (String.IsNullOrEmpty(displayName))
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            if (String.IsNullOrEmpty(botUserName))
            {
                throw new ArgumentNullException(nameof(botUserName));
            }

            if (String.IsNullOrEmpty(botUserId))
            {
                throw new ArgumentNullException(nameof(botUserId));
            }

            if (String.IsNullOrEmpty(botId))
            {
                throw new ArgumentNullException(nameof(botId));
            }

            if (String.IsNullOrEmpty(botName))
            {
                throw new ArgumentNullException(nameof(botName));
            }

            if (String.IsNullOrEmpty(serviceUrl))
            {
                throw new ArgumentNullException(nameof(serviceUrl));
            }

            if (String.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Email = email;
            DisplayName = displayName;
            BotUserId = botUserId;
            BotUserName = botUserName;
            BotId = botId;
            BotName = botName;
            ConversationId = conversationId;
            ChannelId = channelId;
            ServiceUrl = serviceUrl;
            Token = token;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public async Task<HttpStatusCode> Save()
        {
            using (var client = new HttpClient())
            {
                var url = string.Format("{0}/api/SaveBotUserHttpTrigger?code={1}", BaseAddress, FunctionKey);
                var content = new StringContent(this.ToString(), Encoding.UTF8, "application/json");
                var result = await client.PostAsync(url, content);
                var body = await result.Content.ReadAsStringAsync();

                if (!result.IsSuccessStatusCode)
                {
                    await Console.Error.WriteLineAsync($"Error. Failed saving user {result.StatusCode} {body}.");
                }

                if (result.StatusCode == HttpStatusCode.Created)
                {

                    this.DeviceId = body;
                }

                return result.StatusCode;
            }
        }
    }
}