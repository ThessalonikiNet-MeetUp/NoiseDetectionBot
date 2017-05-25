
using Microsoft.Bot.Connector;
using NoiseDetectionBot.Helpers;
using System;
using System.Collections.Generic;
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

            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: conversationId);
            var graphHelper = new GraphHelper(accessToken);
            //graphHelper.Token = accessToken;
            var userinfo = await graphHelper.GetUserInfo();
            var meetingRoomsList = await graphHelper.GetMeetingRoomSuggestions();
            message.Text = $"Hello {userinfo.DisplayName}. ";// {meetingRoomsList.First()} is available. You could continue there!";
            if (meetingRoomsList != null)
            {
                message.Text += "The following meeting rooms are available :\n";
                foreach (var item in meetingRoomsList)
                {
                    message.Text += String.Format("{0}\t{1}", item.DisplayName, item.LocationEmailAddress);
                }

            }
            message.Locale = "en-Us";
            //var reply = context.MakeMessage();
            var animationCard = new HeroCard
            {
                Title = "It seems you're making too much noise",
                Subtitle = "",
                Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                Images = new List<CardImage> { new CardImage("https://media.giphy.com/media/xT5LML6QL8ft5UsC6Q/giphy.gif") },
                //Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "", value: "https://docs.microsoft.com/bot-framework") }
            }.ToAttachment();
            message.Attachments.Add(animationCard);
            //await context.PostAsync(reply);
            //context.Wait(this.MessageReceivedAsync);
            await connector.Conversations.SendToConversationAsync((Activity)message);

        }
    }
}