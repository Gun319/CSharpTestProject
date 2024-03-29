﻿using System.Windows;
using ImageProcessing.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ImageProcessing.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();

            this.Height = SystemParameters.WorkArea.Height / 1.5;
            this.Width = SystemParameters.WorkArea.Width / 1.5;

            this.DataContext = App.Current.Services.GetService<ShellViewModel>();
        }
    }
}
