using System.Windows;

namespace XAMLToImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Conversion.GenerateImage(new TemplateControl(),"jpeg", @"D:\Downloads\TemplateImage.jpeg");
        }
    }
}
