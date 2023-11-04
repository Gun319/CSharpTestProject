using System;
using System.Windows.Controls;
using ImageProcessing.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ImageProcessing.Views
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
