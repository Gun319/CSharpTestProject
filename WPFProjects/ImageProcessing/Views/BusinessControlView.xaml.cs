using System.Windows.Controls;
using ImageProcessing.ViewModels;

namespace ImageProcessing.Views
{
    /// <summary>
    /// BusinessControlView.xaml 的交互逻辑
    /// </summary>
    public partial class BusinessControlView : UserControl
    {
        public BusinessControlView()
        {
            InitializeComponent();

            this.DataContext = new BusinessControlViewModel();
        }
    }
}
