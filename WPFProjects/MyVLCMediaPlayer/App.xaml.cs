using LibVLCSharp.Shared;
using System.Windows;

namespace MyVLCMediaPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Task.Factory.StartNew(() =>
            {
                Core.Initialize();
                Commons.CommonClass.VLCMedia = new LibVLC();
            });
        }
    }
}
