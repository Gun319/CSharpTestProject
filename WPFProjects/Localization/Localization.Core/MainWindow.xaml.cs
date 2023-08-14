using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Localization.Core.Operates;

namespace Localization.Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<KeyValuePair<string, int>>? TimeList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var lan = Thread.CurrentThread.CurrentCulture.Name;
            comLan.SelectedValue = lan;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TimeList = new ObservableCollection<KeyValuePair<string, int>>()
            {
                new KeyValuePair<string, int>("LockTime-OneMinute", 1),
                new KeyValuePair<string, int>("LockTime-FiveMinute", 5),
                new KeyValuePair<string, int>("LockTime-TenMinute", 10),
                new KeyValuePair<string, int>("LockTime-FifteenMinute", 15),
                new KeyValuePair<string, int>("LockTime-ThirtyMinute", 30),
                new KeyValuePair<string, int>("LockTime-OneHour", 60),
                new KeyValuePair<string, int>("LockTime-TwoHour", 120),
                new KeyValuePair<string, int>("LockTime-ThreeHour", 180),
                new KeyValuePair<string, int>("LockTime-Never", 0),
            };

            cmTime.ItemsSource = TimeList;
            cmTime.SelectedValue = TimeList[0];
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems is null) return;

            LanguageOperate.SetLanguage((e.AddedItems[0] as ComboBoxItem)!.Tag.ToString()!);
        }
    }
}
