using System.Windows.Controls;
using ImageProcessing.ViewModels.Business;
using Microsoft.Extensions.DependencyInjection;

namespace ImageProcessing.Views.Business
{
    /// <summary>
    /// ComicView.xaml 的交互逻辑
    /// </summary>
    public partial class ComicView : UserControl
    {
        public ComicView()
        {
            InitializeComponent();

            this.DataContext = App.Current.Services.GetService<ComicViewModel>();
        }
    }
}
