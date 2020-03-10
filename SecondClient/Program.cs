using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace SecondClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // CreateSecondClient();
            StartClient();
            
            Console.ReadLine();
        }

        private static async void StartClient()
        {
            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            
            var options = new MqttClientOptionsBuilder()
                .WithClientId("Client2")
                .WithTcpServer("localhost", 1883)
                .WithCredentials("Hans", "Test")
                // .WithTls()
                .WithCleanSession()
                .Build();
            
            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("test/topic").Build());

                Console.WriteLine("### SUBSCRIBED ###");
            });
            
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                // Console.WriteLine($"+ ClientId = {e.ApplicationMessage.UserProperties.ToString()}");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();

                Task.Run(() => mqttClient.PublishAsync("hello/world"));
            });
            
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("test/topic")
                .WithPayload("2")
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();
            

            await mqttClient.ConnectAsync(options);
            await mqttClient.PublishAsync(message);
        }
    }
}