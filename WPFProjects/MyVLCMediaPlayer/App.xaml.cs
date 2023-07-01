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
            Process[] pList = Process.GetProcessesByName("MyVLCMediaPlayer");

            if (pList.Where(p => p.Id != Environment.ProcessId).Any())
            {
                SwitchToThisWindow(pList.FirstOrDefault().MainWindowHandle, true);

                Environment.Exit(0);
            }

            Startup += App_Startup;
            Exit += App_Exit;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // UI线程未捕获异常处理
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // 非UI线程未捕获异常处理事件
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void App_Exit(object sender, ExitEventArgs e) => Environment.Exit(0);

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                MessageBox.Show($"未处理异常：{e.Exception.Message}", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("发生致命异常，终止执行", "异常", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show($"线程内未处理异常：{e.Exception.Message}", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
                MessageBox.Show("发生致命异常，终止执行", "异常", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show($"捕获未处理异常：{(e.ExceptionObject as Exception).Message}", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            SplashScreen splash = new("/Resources/LaunchPreview.jpg");
            splash.Show(true);

            Task.Factory.StartNew(() =>
            {
                Core.Initialize();
                Commons.CommonClass.VLCMedia = new LibVLC("--avcodec-hw=any");
            });
        }
    }
}
