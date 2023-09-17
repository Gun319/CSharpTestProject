using System.Windows;
using OpenCVSharp.Study.ViewModels;

namespace OpenCVSharp.Study.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Width = SystemParameters.WorkArea.Width / 1.5;
            Height = SystemParameters.WorkArea.Height / 1.5;

            this.DataContext = new MainWindowViewModel();
        }
    }
}
