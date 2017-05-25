namespace NoiseDetectionBot.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.History;
    using Microsoft.Bot.Connector;

    public class DebugActivityLogger : IActivityLogger
    {
        public async Task LogAsync(IActivity activity)
        {
            Trace.TraceInformation($"From={activity.From.Id}. To={activity.Recipient.Id}. Message={activity.AsMessageActivity()?.Text}");
        }
    }
}