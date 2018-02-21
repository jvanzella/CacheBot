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
    public class RootDialog : IDialog<IMessageActivity>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            QueueManager.toId = message.From.Id;
            QueueManager.toName = message.From.Name;
            QueueManager.fromId = message.Recipient.Id;
            QueueManager.fromName = message.Recipient.Name;
            QueueManager.serviceUrl = message.ServiceUrl;
            QueueManager.channelId = message.ChannelId;
            QueueManager.conversationId = message.Conversation.Id;

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
                    var builder = new ServiceBusConnectionStringBuilder(connectionString) {EntityPath = "Requests"};
                    var client = new QueueClient(builder);

                    var redisCommand = new RedisCommand {CommandType = CommandType.ClearAll, DatabaseId = 12};

                    await client.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(redisCommand))));

                    await context.PostAsync("Command to clear redis cache sent");
                }
                catch (Exception ex)
                {
                    await context.PostAsync($"Exception: {ex}. Stack trace: {ex.StackTrace}");
                    context.Wait(MessageReceivedAsync);
                    return;
                }
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}