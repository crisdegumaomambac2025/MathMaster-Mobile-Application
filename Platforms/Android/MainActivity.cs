using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Net;
using Android.Content;
using Android.Widget;

namespace MathMaster;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Check for internet connectivity
        if (!IsNetworkAvailable())
        {
            // Show error message and close the app
            Toast.MakeText(this, "No internet connection. This app requires internet to run.", ToastLength.Long).Show();
            Finish();
            return;
        }
    }

    private bool IsNetworkAvailable()
    {
        ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
        NetworkInfo activeNetworkInfo = connectivityManager.ActiveNetworkInfo;
        return activeNetworkInfo != null && activeNetworkInfo.IsConnected;
    }
}
