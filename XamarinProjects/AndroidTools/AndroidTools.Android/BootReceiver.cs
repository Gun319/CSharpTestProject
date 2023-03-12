using Android.App;
using Android.Content;
using Android.Widget;

namespace AndroidTools.Droid
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted, Intent.ActionReboot })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (intent.Action != null && intent.Action is Intent.ActionBootCompleted)
                {
                    Intent _intent = new Intent();
                    _intent.SetClass(context, typeof(MainActivity));
                    _intent.SetFlags(ActivityFlags.NewTask);
                    context.StartService(_intent);
                }
            }
            catch (System.Exception ex)
            {
                Toast.MakeText(context, ex.Message, ToastLength.Short).Show();
            }
        }
    }
}