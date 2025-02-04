using Android.App;
using Android.Content;
using Android.Content.PM;  
using Android.OS;

namespace MqttApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Iniciar el servicio MQTT en segundo plano
            var intent = new Intent(this, typeof(MqttApp.Platforms.Android.MqttForegroundService));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                ComponentName? componentName = StartForegroundService(intent);
            }
            else
            {
                StartService(intent);
            }
            
        }

    }
}
