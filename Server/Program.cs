using System;
using System.Text;
using MQTTnet;
using MQTTnet.Server;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //
            // var mqttFactory = new MqttFactory();
            // var client = mqttFactory.CreateMqttClient();
            //
            // var options = new MqttClientOptionsBuilder()
            //     .WithClientId(Guid.NewGuid().ToString())
            //     .WithTcpServer("tcp://192.168.1.190:1883")
            //     .Build();
            //
            // var mqttClientAuthenticateResult = client.ConnectAsync(options).Result;

            StartServer();

        }

        private static async void StartServer()
        {
            var mqttServer = new MqttFactory().CreateMqttServer();
            
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithApplicationMessageInterceptor(context =>
                {
                    
                    Console.WriteLine($"Message received from {context.ClientId}, Message is : {Encoding.UTF8.GetString(context.ApplicationMessage.Payload)}");
                    
                    if (context.ApplicationMessage.Topic == "topic/test")
                    {
                        context.ApplicationMessage.Payload = Encoding.UTF8.GetBytes("The server injected payload.");
                    }

                    // It is possible to disallow the sending of messages for a certain client id like this:
                    if (context.ClientId != "Client1")
                    {
                        context.AcceptPublish = false;
                        return;
                    }
                    // It is also possible to read the payload and extend it. For example by adding a timestamp in a JSON document.
                    // This is useful when the IoT device has no own clock and the creation time of the message might be important.
                })
                .WithSubscriptionInterceptor(context =>
                {
                    // if (context.TopicFilter.Topic.StartsWith("admin/foo/bar") && context.ClientId != "theAdmin")
                    // {
                    //     context.AcceptSubscription = false;
                    // }
                    
                    Console.WriteLine($"Subscription received from {context.ClientId}");

                    if (context.TopicFilter.Topic.StartsWith("topic/test") && context.ClientId.StartsWith("Client"))
                    {
                        context.AcceptSubscription = false;
                        context.CloseConnection = true;
                    }
                })
                .Build();
            
            await mqttServer.StartAsync(optionsBuilder);
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
            await mqttServer.StopAsync();
        }
    }
}