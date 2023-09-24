using CommunityToolkit.Mvvm.ComponentModel;

namespace ImageProcessing.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private string title;

        public ViewModelBase()
        {
            Title = "MainWindow";
        }
    }
}
