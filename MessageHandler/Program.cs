using HtmlTemplateCache;
using Messages;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using RedisClient;
using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace MessageHandler
{
    class Program
    {
        QueueClient sendClient;
        QueueClient receiveClient;

        public async Task Run()
        {
            var connectionString = new ServiceBusConnectionStringBuilder(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"]);
            connectionString.EntityPath = ConfigurationManager.AppSettings["RequestsQueueName"];

            receiveClient = new QueueClient(connectionString);
            InitializeReceiver();

            var responseConnectionString = new ServiceBusConnectionStringBuilder(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"]);
            responseConnectionString.EntityPath = ConfigurationManager.AppSettings["ResponsesQueueName"];
            sendClient = new QueueClient(responseConnectionString);

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

                    switch(command.Cache)
                    {
                        case CacheEnum.Redis:
                            await sendClient.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(await ExecuteRedisCommand(command)))));
                            break;
                        case CacheEnum.HtmlTemplate:
                            await sendClient.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(await ExecuteHtmlTemplateCommand(command)))));
                            break;
                    }

                    await receiveClient.CompleteAsync(message.SystemProperties.LockToken);
                },
                new MessageHandlerOptions((e) => LogMessageHandlerException(e)) { AutoComplete = false, MaxConcurrentCalls = 1 });
        }
        
        public async Task<ResponseMessage> ExecuteHtmlTemplateCommand(Command command)
        {
            HtmlTemplateCacheClient htmlTemplateCacheClient = new HtmlTemplateCacheClient();
            try
            {
                await htmlTemplateCacheClient.Clear();
            } catch (Exception ex)
            {
                return new ResponseMessage
                {
                    Status = Status.fail,
                    Error = ex.Message
                };
            }

            return new ResponseMessage
            {
                Status = Status.success
            };
        }

        public async Task<ResponseMessage> ExecuteRedisCommand(Command command)
        {            
            if (command.DatabaseId == null)
            {
                return  new ResponseMessage
                {
                    Status = Status.fail,
                    Error = "No Database Id provided."
                };
            }

            var data = string.Empty;
            RedisCacheService redisCacheService = new RedisCacheService(command);
            try
            {
                switch (command.CommandType)
                {
                    case CommandType.ClearAll:
                        await redisCacheService.Clear();
                        break;

                    case CommandType.GetValue:
                        data = await redisCacheService.Get();
                        break;

                    case CommandType.Remove:
                        await redisCacheService.Remove();
                        break;
                }
            }
            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    Status = Status.fail,
                    Error = ex.Message
                };
            }

            return new ResponseMessage
            {
                Status = Status.success,
                Data = !string.IsNullOrEmpty(data) ? data : null
            };
            
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
