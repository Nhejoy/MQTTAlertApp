using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using MQTTnet.Client;
using MQTTnet;



namespace MqttApp.Platforms.Android
{
    [Service]
    public class MqttForegroundService : Service
    {
        private const int NotificationId = 1001;
        private IMqttClient? _mqttClient;

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            CreateNotificationChannel();

            var notification = new Notification.Builder(this, "mqtt_channel")
                .SetContentTitle("MQTT Servicio Activo")
                .SetContentText("Conectado al broker y escuchando...")
                .SetSmallIcon(Resource.Drawable.icon)
                .Build();

            StartForeground(NotificationId, notification);
            StartMqttConnection();

            return StartCommandResult.Sticky;
        }

        private async void StartMqttConnection()
        {
            try
            {
                var factory = new MqttFactory();
                _mqttClient = factory.CreateMqttClient();

                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer("broker.hivemq.com", 1883)
                    .WithClientId("MqttAndroidClient")
                    .Build();

                _mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    Log.Debug("MqttForegroundService", $"Mensaje recibido: {message}");

                    if (message == "1")
                    {
                        ShowNotification("Mensaje recibido", "El tópico tiene el mensaje '1'");
                    }
                };

                await _mqttClient.ConnectAsync(options);
                await _mqttClient.SubscribeAsync("Ivan/prueba/Csharp");
            }
            catch (Exception ex)
            {
                Log.Error("MqttForegroundService", $"Error en la conexión MQTT: {ex.Message}");
            }
        }

        private void ShowNotification(string title, string message)
        {
            var notification = new Notification.Builder(this, "mqtt_channel")
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.icon)
                .Build();

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(2001, notification);
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    "mqtt_channel",
                    "MQTT Notifications",
                    NotificationImportance.Default)
                {
                    Description = "Canal para notificaciones MQTT"
                };

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager?.CreateNotificationChannel(channel);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (_mqttClient?.IsConnected ?? false)
            {
                _mqttClient.DisconnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            Log.Debug("MqttForegroundService", "Service destroyed.");
        }
    }
}
