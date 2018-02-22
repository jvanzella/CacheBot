using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Bot.Connector;

namespace CacheBot
{
    public class QueueManager
    {
//        public static string toId;
        private const string ConversationId = "B9CP104JZ:T9CNR3GRK:C9CJL2R28"; // #general
        private const string ChannelId = "slack";
        private const string ServiceUrl = "https://slack.botframework.com/";
        private const string FromName = "cachebot";
        private const string FromId = "B9CP104JZ:T9CNR3GRK"; // cachebot's id
//        public static string toName;

        public static void RegisterQueue()
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            if (connectionString == null)
            {
                throw new Exception("Connection String not there");
            }
            
                var builder = new ServiceBusConnectionStringBuilder(connectionString) { EntityPath = "Responses" };
                var client = new QueueClient(builder);

                client.RegisterMessageHandler(async (queueMessage, token) =>
                {
//                    if (toId == null)
//                    {
//                        throw new Exception("Cannot send message if I haven't received one. We should probably fix this.");
//                    }

                    // Use the data stored previously to create the required objects.
//                    var userAccount = new ChannelAccount(toId, toName);
                    var botAccount = new ChannelAccount(FromId, FromName);
                    var connector = new ConnectorClient(new Uri(ServiceUrl));

                    // Create a new message.
                    IMessageActivity message = Activity.CreateMessageActivity();
//                    if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(channelId))
//                    {
                        // If conversation ID and channel ID was stored previously, use it.
                        message.ChannelId = ChannelId;
//                    }
//                    else
//                    {
//                        // Conversation ID was not stored previously, so create a conversation. 
//                        // Note: If the user has an existing conversation in a channel, this will likely create a new conversation window.
//                        conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, null, cancellationToken: token)).Id;
//                    }

                    // Set the address-related properties in the message and send the message.
                    message.From = botAccount;
//                    message.Recipient = userAccount;
                    message.Conversation = new ConversationAccount(id: ConversationId);
                    message.Text = "The cache was successfully cleared!";
                    message.Locale = "en-us";
                    await connector.Conversations.SendToConversationAsync((Activity)message, token);

                    await client.CompleteAsync(queueMessage.SystemProperties.LockToken);
                }, new MessageHandlerOptions(LogMessageHandlerException) { AutoComplete = false, MaxConcurrentCalls = 1 });
        }

        private static Task LogMessageHandlerException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.FromResult(0);
        }
    }
}