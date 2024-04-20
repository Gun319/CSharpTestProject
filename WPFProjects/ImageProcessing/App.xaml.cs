using System;
using System.Windows;
using ImageProcessing.ViewModels;
using ImageProcessing.ViewModels.Business;
using ImageProcessing.ViewModels.Controls;
using ImageProcessing.Views.Business;
using ImageProcessing.Views.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ImageProcessing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();

            this.InitializeComponent();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ItemMenuView>();
            services.AddSingleton<ImageSynthesisVideoView>();
            services.AddSingleton<MatchTemplateView>();
            services.AddSingleton<ComicView>();
            services.AddSingleton<DocumentCorrectionView>();

            services.AddSingleton<ShellViewModel>();
            services.AddSingleton<ItemMenuViewModel>();
            services.AddSingleton<ImageSynthesisVideoViewModel>();
            services.AddSingleton<MatchTemplateViewModel>();
            services.AddSingleton<ComicViewModel>();
            services.AddSingleton<DocumentCorrectionViewModel>();


            return services.BuildServiceProvider();
        }
    }
}
