
using Microsoft.Bot.Connector;
using NoiseDetectionBot.Helpers;
using System;
using System.Collections.Generic;
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

            IMessageActivity message = Activity.CreateMessageActivity();
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
            var animationCard = new HeroCard
            {
                Title = $"Hello { userInfo.Item2.DisplayName }.",
                Subtitle = $" It seems you're making a lot of noise!\n",
                Text = "",
                Images = new List<CardImage> { new CardImage("https://media.giphy.com/media/xT5LML6QL8ft5UsC6Q/giphy.gif") },
                //Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "", value: "https://docs.microsoft.com/bot-framework") }
            }.ToAttachment();
          //  if (userInfo.Item1)
           // {
                var meetingRoomsList = await graphHelper.GetMeetingRoomSuggestions();
                if (meetingRoomsList.Any())
                {
                    animationCard = new HeroCard
                    {
                        Title = $"Hello { userInfo.Item2.DisplayName }.",
                        Subtitle = $" It seems you're making a lot of noise!\n",
                        Text = $"The following meeting rooms are available : {string.Join(",", meetingRoomsList.Select(x => x.DisplayName).ToList()) }. Would you like to continue there?",
                        Images = new List<CardImage> { new CardImage("https://media.giphy.com/media/xT5LML6QL8ft5UsC6Q/giphy.gif") },
                        //Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "", value: "https://docs.microsoft.com/bot-framework") }
                    }.ToAttachment();

                }
          //  }


            message.Attachments.Add(animationCard);
           

            //await connector.Conversations.SendToConversationAsync((Activity)message);
            await connector.Conversations.SendToConversationAsync((Activity)message);
        }
    }
}