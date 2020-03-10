using System;
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
            await mqttServer.StartAsync(new MqttServerOptions());
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
            await mqttServer.StopAsync();
        }
    }
}