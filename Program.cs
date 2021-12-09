using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace ServiceBusListener
{
    class Program
    {
        //const string ServiceBusConnectionString = "Endpoint=sb://scramtest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=w1DOmuMLlKY7bwG/YF6+O3/KhWF4HUUV7YyvdXx639I=";
        //const string ServiceBusConnectionString = "Endpoint=sb://scramstage.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YNJcB1DOay0p7Kgl6J9Q6Uq9mbkEsWSeexJV9jfO40I=";
        const string ServiceBusConnectionString = "Endpoint=sb://twentyfourseven.servicebus.usgovcloudapi.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Lv1yZPrPStUvb0TkwCRpufusONaPk0uCPmgJSM7XJ1w=";
        //const string TopicName = "24x7-monitored-day-fee";
        const string TopicName = "clientmonitordaychanges";
        //const string SubscriptionName = "md-pub-dev";
        //const string SubscriptionName = "md-pub-stage-save";
        //const string SubscriptionName = "sanity-check";
        const string SubscriptionName = "24x7SanityCheck";
        static ISubscriptionClient _subscriptionClient;

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            _subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);

            Console.WriteLine("Receiving messages...");
            Console.WriteLine("Press any key to exit");

            RegisterOnMessageHandlerAndReceiveMessage();

            Console.ReadKey();

            await _subscriptionClient.CloseAsync();
        }

        private static void RegisterOnMessageHandlerAndReceiveMessage()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var messageBody = Encoding.UTF8.GetString(message.Body);
            //var monitorDayMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<MonitorDayMessage>(messageBody);
            var removeReplaceMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<ScramRemoveReplaceMessage>(messageBody);

            byte[] encodedText = Encoding.Unicode.GetBytes($"{removeReplaceMessage.OldServiceTypeLevel},{removeReplaceMessage.NewServiceTypeLevel},{removeReplaceMessage.ClientId},{removeReplaceMessage.BaseAccountId},{removeReplaceMessage.MonitorDate},{removeReplaceMessage.ServiceTypeLevel},{removeReplaceMessage.ServiceOptionLevel},{removeReplaceMessage.Added},{removeReplaceMessage.Removed},{removeReplaceMessage.UpdatedDateTime},{removeReplaceMessage.CurrentServiceTypeLevel},{removeReplaceMessage.BillableStateLevel}\n");

            using (FileStream fileStream = new FileStream("removeReplaceMessages.csv", FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await fileStream.WriteAsync(encodedText, 0, encodedText.Length);
            }

            Console.WriteLine($"Received message: Sequence: {message.SystemProperties.SequenceNumber}\n EnqueuedTimeUtc:{message.SystemProperties.EnqueuedTimeUtc}\n Body:{Encoding.UTF8.GetString(message.Body)}");
            
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
