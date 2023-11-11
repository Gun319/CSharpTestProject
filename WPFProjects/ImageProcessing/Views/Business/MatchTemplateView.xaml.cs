using System;
using System.Windows.Controls;
using ImageProcessing.ViewModels.Business;
using Microsoft.Extensions.DependencyInjection;

namespace ImageProcessing.Views.Business
{
    /// <summary>
    /// MatchTemplateView.xaml 的交互逻辑
    /// </summary>
    public partial class MatchTemplateView : UserControl
    {
        public MatchTemplateView()
        {
            InitializeComponent();

            this.DataContext = App.Current.Services.GetService<MatchTemplateViewModel>();
        }
    }
}
