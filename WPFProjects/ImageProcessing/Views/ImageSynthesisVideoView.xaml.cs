using System.Windows.Controls;
using ImageProcessing.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ImageProcessing.Views
{
    /// <summary>
    /// ImageSynthesisVideoView.xaml 的交互逻辑
    /// </summary>
    public partial class ImageSynthesisVideoView : UserControl
    {
        public ImageSynthesisVideoView()
        {
            InitializeComponent();

            this.DataContext = App.Current.Services.GetService<ImageSynthesisVideoViewModel>();
        }
    }
}
