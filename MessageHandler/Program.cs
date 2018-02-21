using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Messages;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MessageHandler
{
    class Program
    {
        QueueClient sendClient;
        QueueClient receiveClient;

        public async Task Run()
        {
            var connectionString = new ServiceBusConnectionStringBuilder(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"]);
            connectionString.EntityPath = ConfigurationManager.AppSettings["QueueName"];

            receiveClient = new QueueClient(connectionString);
            InitializeReceiver();

            while (true) ;

            await this.receiveClient.CloseAsync();
        }

        void InitializeReceiver()
        {
            // register the RegisterMessageHandler callback
            receiveClient.RegisterMessageHandler(
                async (message, cancellationToken) =>
                {                    
                    var body = Encoding.UTF8.GetString(message.Body);

                    var command = JsonConvert.DeserializeObject<Command>(body);
                    
                    Console.WriteLine(command);

                    await receiveClient.CompleteAsync(message.SystemProperties.LockToken);
                },
                new MessageHandlerOptions((e) => LogMessageHandlerException(e)) { AutoComplete = false, MaxConcurrentCalls = 1 });
        }

        private Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            Console.WriteLine("Exception: \"{0}\" {0}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath);
            return Task.CompletedTask;
        }

        public static int Main(string[] args)
        {
            try
            {
                var app = new Program();
                app.Run().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }
            return 0;
        }
    }
}
