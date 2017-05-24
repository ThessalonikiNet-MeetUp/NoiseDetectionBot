using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleAADV2Bot.Models;
using Newtonsoft.Json;

namespace Bot.Test
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void Serializer()
        {
            var user = new User("email", "displayName", "botUserName", "botUserId", "botId", "botName", "serviceUrl", "token", "conversationId", "channelId");
            Console.WriteLine(user.ToString());
        }

        [TestMethod]
        public void SaveTest()
        {
            User.BaseAddress = "https://noisedetectionfunctions.azurewebsites.net";
            User.FunctionKey = "tiBQsCljXzY0gAM5Zc0EkqzctlUc9wWa29VFtp3bfd7FZ/tfRGrXDw==";

            var user = new User("email", "displayName", "botUserName", "botUserId", "botId", "botName", "serviceUrl", "token", "conversationId", "channelId");
            var id = user.Save().Result;
            Console.WriteLine(id);
            Assert.IsNotNull(id);
        }
    }
}
