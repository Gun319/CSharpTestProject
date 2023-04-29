using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace MyVLCMediaPlayer.ChildWindows
{
    /// <summary>
    /// NetworkUrlWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NetworkUrlWindow : Window
    {
        /// <summary>
        /// 匹配数字
        /// </summary>
        private readonly Regex regex = new("/[^\\d]+");

        public NetworkUrlWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtNetworkUrl.Focus();
            SPMore.Visibility = Visibility.Collapsed;
        }

        private void TxtCancel_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ChkMore_Checked(object sender, RoutedEventArgs e)
        {
            SPMore.Visibility = Visibility.Visible;
        }

        private void ChkMore_Unchecked(object sender, RoutedEventArgs e)
        {
            SPMore.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 指定网络地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            if (TxtNetworkUrl.Text.Trim().Length != 0)
            {
                Commons.CommonClass.NetworkUrl = TxtNetworkUrl.Text.Trim();
                Commons.CommonClass.CacheTime = TxtCancel.Text.Trim().Length != 0 ? Convert.ToInt32(TxtCancel.Text.Trim()) : 0;
                DialogResult = true;
                return;
            }
            else
            {
                ErrorPrompt.Content = "请输入网络地址";
            }
        }

        /// <summary>
        /// 关闭弹窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TxtNetworkUrl_GotFocus(object sender, RoutedEventArgs e)
        {
            ErrorPrompt.Content = null;
        }

        /// <summary>
        /// 双击全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtNetworkUrl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TxtNetworkUrl.SelectAll();
        }
    }
}
