using MqttApp.Services;
using System.Diagnostics;

namespace MqttApp
{
    public partial class MainPage : ContentPage
    {

        private readonly MqttService _mqttService;

        public MainPage(MqttService mqttService)
        {
            InitializeComponent();
            _mqttService = mqttService;

            _mqttService.ConnectionStatusChanged += OnConnectionStatusChanged;
            _mqttService.MessageReceived += OnMessageReceived;
        }

        private void OnConnectionStatusChanged(bool isConnected)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ConnectionStatusLabel.Text = isConnected ? "Estado: Conectado" : "Estado: Desconectado";
            });
        }


        private async void ConnectButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await _mqttService.ConnectAsync("broker.hivemq.com", 1883);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void OnMessageReceived(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Muestra el contenido del mensaje en un Label.
                MessageContentLabel.Text = message;
            });
        }

        private async void SubscribeButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await _mqttService.SubscribeAsync("Nhejoy/prueba/Csharp"); // Cambia "mi/topico" al tópico deseado.
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }



    }

}
