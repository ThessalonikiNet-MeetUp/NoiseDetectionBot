using Newtonsoft.Json;
using System;
using System.Configuration;
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

        public string Id { get; internal set; }

        [DataMember]
        public string UserId { get; internal set; }

        [DataMember]
        public string UserName { get; internal set; }

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

        public User(
            string userId, 
            string username, 
            string botId, 
            string botName, 
            string serviceUrl,
            string token,
            string conversationId = null, 
            string channelId = null)
        {
            if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username));
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

            UserId = userId;
            UserName = username;
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

        public async Task<string> Save()
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
                    return null;
                }
                return body;
            }
        }
    }
}