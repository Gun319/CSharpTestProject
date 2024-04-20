using System.Windows.Controls;
using ImageProcessing.ViewModels.Business;
using Microsoft.Extensions.DependencyInjection;

namespace ImageProcessing.Views.Business
{
    /// <summary>
    /// DocumentCorrectionView.xaml 的交互逻辑
    /// </summary>
    public partial class DocumentCorrectionView : UserControl
    {
        public DocumentCorrectionView()
        {
            InitializeComponent();

            this.DataContext = App.Current.Services.GetService<DocumentCorrectionViewModel>();
        }
    }
}
