// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.
namespace SampleAADV2Bot.Dialogs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AuthBot;
    using AuthBot.Dialogs;
    using AuthBot.Models;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using SampleAADV2Bot.Controllers;
    using SampleAADV2Bot.Models;
    using System.Collections.Generic;
    using SampleAADV2Bot.Helpers;
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

                await context.PostAsync("echo");
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
                   
                    await ConversationStarter.Resume(messageDictionary["conversationId"], messageDictionary["channelId"], messageDictionary["recipientId"], messageDictionary["recipientName"], message.Recipient.Id, message.Recipient.Name, messageDictionary["serviceUrl"], messageDictionary["token"]);//context.PostAsync("echo");
                    var reply = context.MakeMessage();
                    var animationCard = new AnimationCard
                    {
                        Title = "Could you please be more quiet?",
                        Subtitle = "",
                        Image = new ThumbnailUrl
                        {
                            Url = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png"
                        },
                        Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "https://media.giphy.com/media/xT5LML6QL8ft5UsC6Q/giphy.gif"
                    }
                }
                    }.ToAttachment();
                    reply.Attachments.Add(animationCard);
                    await context.PostAsync(reply);
                    context.Wait(this.MessageReceivedAsync);
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Error parsing NDBDATA. Exception={e.Message}");
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

            var user = new User(
                userInfo.Mail,
                userInfo.DisplayName,
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
