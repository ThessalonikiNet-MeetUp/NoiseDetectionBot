using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NoiseDetectionBot.Models
{
    [DataContract]
    public class Device
    {
        [DataMember]
        public int Id { get; internal set; }

        [DataMember]
        public string Name { get; internal set; }

        [DataMember]
        public int UserId { get; internal set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /*
        public async static Task<Device> Get(string deviceId)
        {
            var baseAddress = ConfigurationManager.AppSettings["Endpoint.BaseAddress"];
            var requestUri = ConfigurationManager.AppSettings["Endpoint.DeviceDataStore"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                var result = await client.GetAsync(requestUri);

                if (!result.IsSuccessStatusCode)
                {
                    await Console.Error.WriteLineAsync($"Error. Failed saving user {result.StatusCode}");
                }

                return result.IsSuccessStatusCode;
            }
        }*/
    }
}