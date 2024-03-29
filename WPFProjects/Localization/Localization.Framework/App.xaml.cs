﻿using System.Windows;

namespace Localization.Framework
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Operates.LanguageOperate.SetLanguage();
        }
    }
}
