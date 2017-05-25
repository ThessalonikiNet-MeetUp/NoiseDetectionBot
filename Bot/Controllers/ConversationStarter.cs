
using Microsoft.Bot.Connector;
using NoiseDetectionBot.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NoiseDetectionBot.Controllers
{
    public class ConversationStarter
    {
        public static async Task Resume(string conversationId, string channelId, string toId, string toName, string fromId,string fromName, string serviceUrl,string accessToken)
        {
            var userAccount = new ChannelAccount(toId, toName);
            var botAccount = new ChannelAccount(fromId, fromName);
            var connector = new ConnectorClient(new Uri(serviceUrl));

            var message = Activity.CreateMessageActivity();
            if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(channelId))
            {
                message.ChannelId = channelId;
            }
            else
            {
                conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
            }

            message.Locale = "en-us";
            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: conversationId);

            var graphHelper = new GraphHelper(accessToken);
            var userInfo = await graphHelper.GetUserInfo();
            Random rnd = new Random();
            int fileindex = rnd.Next(1, 7);
            string fileurl = "https://raw.githubusercontent.com/ThessalonikiNet-MeetUp/NoiseDetectionBot/master/Bot/Images/shh" + fileindex +".gif";
            
            var animationCard = new HeroCard
            {
                Title = $"Hello { userInfo.Item2.DisplayName }.",
                Subtitle = $" It seems you're making a lot of noise!\n",
                Text = "",
                Images = new List<CardImage> { new CardImage(fileurl) },
            }.ToAttachment();

            if (userInfo.Item1)
            {
                var meetingRoomsList = await graphHelper.GetMeetingRoomSuggestions();

                Trace.TraceInformation($"GetMeetingRoomSuggestions: Found {meetingRoomsList.Count} rooms.");
                if (meetingRoomsList.Any())
                {
                    animationCard = new HeroCard
                    {
                        Title = $"Hello { userInfo.Item2.DisplayName }.",
                        Subtitle = $" It seems you're making a lot of noise!\n",
                        Text = $"The following meeting rooms are available : {string.Join(",", meetingRoomsList.Select(x => x.DisplayName).ToList()) }. Would you like to continue there?",
                        Images = new List<CardImage> { new CardImage(fileurl) },
                    }.ToAttachment();
                }
            }

            message.Attachments.Add(animationCard);
           
            await connector.Conversations.SendToConversationAsync((Activity)message);
        }
    }
}