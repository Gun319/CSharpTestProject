using System;
using System.Windows;

namespace OpenCVSharp.Study
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
             Environment.Exit(0);

            base.OnExit(e);
        }
    }
}
