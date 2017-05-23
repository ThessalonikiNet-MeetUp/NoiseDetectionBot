using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleAADV2Bot.Models;

namespace Bot.Test
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void Serializer()
        {
            var user = new User("id", "name", "botId", "botName", "conversationId", "channelId", "serviceUrl");
            Console.WriteLine(user.ToString());
        }

        [TestMethod]
        public void SaveTest()
        {
            var user = new User("userId", "username", "botId", "botName", "serviceUrl", "token", "conversationId", "channelId");
            var id = user.Save().Result;
            Console.WriteLine(id);
            Assert.IsNotNull(id);
        }
    }
}
