using Android.App;
using Android.Content.PM;
using Android.OS;


namespace fairdao.portal
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : fairdao.portal.FairdaoMainActivity
    {
    }
}
