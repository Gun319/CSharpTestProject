using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Localization.Framework.Operates;

namespace Localization.Framework
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> list = new List<string>();
        ObservableCollection<KeyValuePair<string, int>> TimeList { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;

            for (int i = 0; i < 2000000; i++)
            {
                list.Add(string.Concat("item", i));
            }

            var lan = Thread.CurrentThread.CurrentCulture.Name;
            comLan.SelectedValue = lan;
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

            LanguageOperate.SetLanguage((e.AddedItems[0] as ComboBoxItem).Tag.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VirtualizingListView.ItemsSource = list;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ImageShowe();
        }

        #region 图片异步加载

        int i;
        private List<ListBoxTest> strings;
        private ObservableCollection<ListBoxTest> lists = new ObservableCollection<ListBoxTest>();
        private void ImageShowe()
        {
            strings = LoadDir($@"{AppDomain.CurrentDomain.BaseDirectory}\Images");
            lists.Clear();
            i = 0;
            lb.ItemsSource = lists;
            lb.Dispatcher.BeginInvoke(DispatcherPriority.Background, new AddItemDelegate(AddItem));
        }

        private delegate void AddItemDelegate();
        private void AddItem()
        {
            if (i < strings.Count)
            {
                lists.Add(strings[i++]);
                lb.Dispatcher.BeginInvoke(DispatcherPriority.Background, new AddItemDelegate(AddItem));
            }
        }

        private List<ListBoxTest> LoadDir(string dirpath)
        {
            List<ListBoxTest> strs = new List<ListBoxTest>();
            if (Directory.Exists(dirpath))
            {
                strs.AddRange(new DirectoryInfo(dirpath).GetFiles("*.jpg").Select(item => new ListBoxTest
                {
                    ImageSource = item.FullName
                }));
            }
            return strs;
        }

        public class ListBoxTest
        {
            public string ImageSource { get; set; }
        }

        #endregion

        private void VirtualizingListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
