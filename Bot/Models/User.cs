using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SampleAADV2Bot.Models
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Id { get; internal set; }

        [DataMember]
        public string Name { get; internal set; }

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
            string id, 
            string name, 
            string botId, 
            string botName, 
            string serviceUrl,
            string token,
            string conversationId = null, 
            string channelId = null)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
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

            Id = id;
            Name = name;
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

        public async Task<bool> Save()
        {
            var baseAddress = ConfigurationManager.AppSettings["UserDataStore.BaseAddress"];
            var requestUri = ConfigurationManager.AppSettings["UserDataStore.RequestUri"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                var content = new StringContent(this.ToString());
                var result = await client.PostAsync(requestUri, content);

                if (!result.IsSuccessStatusCode)
                {
                    await Console.Error.WriteLineAsync($"Error. Failed saving user {result.StatusCode}");
                }

                return result.IsSuccessStatusCode;
            }
        }
    }
}