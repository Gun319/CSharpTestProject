using System;
using System.Windows.Controls;
using ImageProcessing.ViewModels.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ImageProcessing.Views.Controls
{
    /// <summary>
    /// ItemMenuView.xaml 的交互逻辑
    /// </summary>
    public partial class ItemMenuView : UserControl
    {
        public ItemMenuView()
        {
            InitializeComponent();

            this.DataContext = App.Current.Services.GetService<ItemMenuViewModel>();
        }
    }
}
