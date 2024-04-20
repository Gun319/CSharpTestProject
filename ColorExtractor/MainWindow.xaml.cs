using System.Windows;
using System.Windows.Media;

namespace ColorExtractor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GlobalMouseHook _mouseHook;
        public MainWindow()
        {
            InitializeComponent();

            _mouseHook = new GlobalMouseHook();
        }

        private void MouseHook_MouseReleased(object sender, Point point)
        {
            Color color = ScreenColorPicker.GetColorAt((int)point.X, (int)point.Y);
            Dispatcher.Invoke(() =>
            {
                HEX_Color.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                RGB_Color.Text = $"{color.R} {color.G} {color.B}";

                // 用来展示颜色的Rectangle
                colorDisplay.Fill = new SolidColorBrush(color);
                if (_mouseHook != null)
                {
                    _mouseHook.ReleaseHook();
                    _mouseHook.MouseReleased -= MouseHook_MouseReleased;
                    _mouseHook = null;
                }
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_mouseHook != null)
            {
                _mouseHook.ReleaseHook();
                _mouseHook.MouseReleased -= MouseHook_MouseReleased;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _mouseHook = new GlobalMouseHook();

            _mouseHook.MouseReleased += MouseHook_MouseReleased;
            _mouseHook.SetHook();
        }
    }
}