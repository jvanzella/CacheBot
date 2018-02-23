using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using CacheBot.Tools;
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
//            QueueManager.toId = message.From.Id; // If marrion sent the message this identifies him
//            QueueManager.toName = message.From.Name;
//            QueueManager.fromId = message.Recipient.Id;
//            QueueManager.fromName = message.Recipient.Name;
//            QueueManager.serviceUrl = message.ServiceUrl;
//            QueueManager.channelId = message.ChannelId;
//            QueueManager.conversationId = message.Conversation.Id;

            var activity = await result as Activity;

            var parser = new BotCommandParser();
            var command = parser.Parse(activity.Text);
            if (command != null)
            {
                if (activity.Text.Contains("givemeids"))
                {
                    await context.PostAsync(
                        $@"toId: {message.From.Id}, 
                        ToName: {message.From.Name}
                        fromId: {message.Recipient.Id}
                        fromName: {message.Recipient.Name}
                        serviceUrl: {message.ServiceUrl}
                        channelId: {message.ChannelId}
                        conversationId: {message.Conversation.Id}");
                    context.Wait(MessageReceivedAsync);
                    return;
                }


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

                    var obj = JsonConvert.SerializeObject(command);
                    await client.SendAsync(new Message(Encoding.UTF8.GetBytes(obj)));

                    await context.PostAsync("Command sent! Will let you know how it goes.");
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
