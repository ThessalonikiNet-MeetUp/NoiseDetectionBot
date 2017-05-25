// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.
namespace NoiseDetectionBot.Dialogs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AuthBot;
    using AuthBot.Dialogs;
    using AuthBot.Models;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using NoiseDetectionBot.Controllers;
    using NoiseDetectionBot.Models;
    using System.Collections.Generic;
    using NoiseDetectionBot.Helpers;
    using System.Net;
    using System.Web.Script.Serialization;
    using System.Diagnostics;

    [Serializable]
    public class ActionDialog : IDialog<string>
    {
        private string accessToken;
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task TokenSample(IDialogContext context)
        {
            //endpoint v2
            var accessToken = await context.GetAccessToken(AuthSettings.Scopes);

            if (string.IsNullOrEmpty(accessToken))
            {
                return;
            }

            await context.PostAsync($"Your access token is: {accessToken}");
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;
            if (message.Text == "logon")
            {
                if (string.IsNullOrEmpty(await context.GetAccessToken(AuthSettings.Scopes)))
                {
                    await context.Forward(new AzureAuthDialog(AuthSettings.Scopes), this.ResumeAfterAuth, message, CancellationToken.None);
                }
                else
                {
                    await TokenSample(context);
                }
            }
            else if (message.Text == "echo")
            {
                var reply = context.MakeMessage();
                var animationCard = new HeroCard
                {
                    Title = "It seems you're making too much noise",
                    Subtitle = "",
                    Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                    Images = new List<CardImage> { new CardImage("https://media.giphy.com/media/xT5LML6QL8ft5UsC6Q/giphy.gif") },
                    //Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "", value: "https://docs.microsoft.com/bot-framework") }
                }.ToAttachment();
                reply.Attachments.Add(animationCard);
                await context.PostAsync(reply);
                context.Wait(this.MessageReceivedAsync);
            }
            else if (message.Text == "token")
            {
                await TokenSample(context);
            }
            else if (message.Text.StartsWith("NDBDATA"))
            {
                var messageinfo = message.Text.Split(';');

                try
                {
                    var messageDictionary = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(messageinfo[1]);
                    var conversationId = messageDictionary["conversationId"];
                    var channelId = messageDictionary["channelId"];
                    var recipientId = messageDictionary["recipientId"];
                    var recipientName = messageDictionary["recipientName"];
                    var serviceUrl = messageDictionary["serviceUrl"];
                    var token = messageDictionary["token"];

                    Trace.TraceInformation($"ActionDialog: Sending notification to {recipientName}.");
                    await ConversationStarter.Resume(conversationId, channelId, recipientId, recipientName, message.Recipient.Id, message.Recipient.Name, serviceUrl, token);
                    Trace.TraceInformation($"ActionDialog: Notification to {recipientName}.");
                }
                catch (Exception e)
                {
                    Trace.TraceError($"ActionDialog: Error parsing NDBDATA. Exception={e.Message}");
                }

                context.Wait(this.MessageReceivedAsync);
            }
            else if (message.Text == "logout")
            {
                await context.Logout();
                context.Wait(this.MessageReceivedAsync);
            }
            else
            {
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterAuth(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;
            await context.PostAsync(message);

            accessToken = await context.GetAccessToken(AuthSettings.Scopes);

            var graphHelper = new GraphHelper(accessToken);
            var userInfo = await graphHelper.GetUserInfo();

            var userEmail = userInfo.Item1 ? userInfo.Item2.Mail : String.Empty;
            var displayName = userInfo.Item1 ? userInfo.Item2.DisplayName : String.Empty;

            var user = new User(
                userEmail,
                displayName,
                context.Activity.From.Id,
                context.Activity.From.Name,
                context.Activity.Recipient.Id,
                context.Activity.Recipient.Name,
                context.Activity.ServiceUrl,
                accessToken,
                context.Activity.Conversation.Id,
                context.Activity.ChannelId);
            var statusCode = await user.Save();

            switch (statusCode)
            {
                case HttpStatusCode.OK:
                    await context.PostAsync($"Welcome back {user.DisplayName}! We're always listening....");
                    break;
                case HttpStatusCode.Created:
                    await context.PostAsync($"Please enter the following into your listening device: {user.DeviceId}");
                    break;
                default:
                    await context.PostAsync($"Error, please try again");
                    break;
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}


//*********************************************************
//
//AuthBot, https://github.com/microsoftdx/AuthBot
//
//Copyright (c) Microsoft Corporation
//All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// ""Software""), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:




// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.




// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//*********************************************************
