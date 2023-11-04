using System;
using System.Windows;
using ImageProcessing.ViewModels;
using ImageProcessing.Views;
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


        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddScoped<ImageSynthesisVideoView>();
            services.AddScoped<MatchTemplateView>();

            services.AddSingleton<ShellViewModel>();
            services.AddScoped<ImageSynthesisVideoViewModel>();
            services.AddScoped<MatchTemplateViewModel>();


            return services.BuildServiceProvider();
        }
    }
}
