using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;

namespace MqttApp.Services
{
    public class MqttService
    {
        private IMqttClient _mqttClient;

        public event Action<bool> ConnectionStatusChanged; // Para notificar el estado de conexión.
        public event Action<string> MessageReceived;

        public MqttService()
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            // Suscríbete a eventos de conexión/desconexión.
            _mqttClient.ConnectedAsync += async e =>
            {
                ConnectionStatusChanged?.Invoke(true); // Notifica cuando se conecta.
            };

            _mqttClient.DisconnectedAsync += async e =>
            {
                ConnectionStatusChanged?.Invoke(false); // Notifica cuando se desconecta.
            };

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                MessageReceived?.Invoke(payload);
            };


        }

        public async Task ConnectAsync(string brokerAddress, int port)
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerAddress, port)
                .Build();

            await _mqttClient.ConnectAsync(options);
        }

        public async Task DisconnectAsync()
        {
            await _mqttClient.DisconnectAsync();
        }

        public async Task SubscribeAsync(string topic)
        {
            await _mqttClient.SubscribeAsync(topic);
        }

        public bool IsConnected => _mqttClient.IsConnected;
    }
}
