using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleAADV2Bot.Models;

namespace Bot.Test
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var user = new User("id", "name", "botId", "botName", "conversationId", "channelId", "serviceUrl");
            Console.WriteLine(user.ToString());
        }
    }
}
