using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoiseDetectionBot.Models;

namespace Bot.Test
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void UserSave()
        {
            var user = new User("email", "displayName", "botUserName", "botUserId", "botId", "botName", "serviceUrl", "token-new", "conversationId", "channelId");
            var statusCode = user.Save().Result;
            Console.WriteLine(statusCode);
            Console.WriteLine(user.DeviceId);
        }
    }
}
