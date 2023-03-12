using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidTools.Contracts;
using AndroidTools.Droid.Contracts;
using Xamarin.Forms;

namespace AndroidTools.Droid
{
    [Activity(Label = "AndroidTools", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Instance = this;
            RegisterTypes();

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void RegisterTypes()
        {
            DependencyService.Register<IAutoOnOff, AutoOnOff>();
            DependencyService.Register<IInstall, Install>();
        }
    }
}