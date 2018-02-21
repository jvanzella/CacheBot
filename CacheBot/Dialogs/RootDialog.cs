using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Messages;
using Microsoft.Azure.ServiceBus;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace CacheBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if(activity.Text.StartsWith("@cachebot", StringComparison.InvariantCultureIgnoreCase))
            {
                var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
                if (connectionString == null)
                {
                    await context.PostAsync("Configuration is wrong");
                    context.Wait(MessageReceivedAsync);
                    return;
                }

                try
                {
                    var builder = new ServiceBusConnectionStringBuilder(connectionString) {EntityPath = "Queue"};
                    var client = new QueueClient(builder);

                    var redisCommand = new Command {CommandType = CommandType.ClearAll, DatabaseId = 12, Cache = CacheEnum.Redis};

                    await client.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(redisCommand))));

                }
                catch (Exception ex)
                {
                    await context.PostAsync($"Exception: {ex}. Stack trace: {ex.StackTrace}");
                    context.Wait(MessageReceivedAsync);
                    return;
                }

                // calculate something for us to return
                var length = (activity?.Text ?? string.Empty).Length;

                // return our reply to the user
                await context.PostAsync($"You sent {activity?.Text} which was {length} characters");
            }


            context.Wait(MessageReceivedAsync);
        }
    }
}