using LibVLCSharp.Shared;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace MyVLCMediaPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 窗口切换至最前
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="fAltTab">是否切换至最前</param>
        [DllImport("user32.dll")]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        public App()
        {
            InitializeComponent();

            Process[] pList = Process.GetProcessesByName("MyVLCMediaPlayer");

            if (pList.Where(p => p.Id != Environment.ProcessId).Any())
            {
                SwitchToThisWindow(pList.FirstOrDefault().MainWindowHandle, true);

                Environment.Exit(0);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Core.Initialize();
                Commons.CommonClass.VLCMedia = new LibVLC();
            });
        }
    }
}
