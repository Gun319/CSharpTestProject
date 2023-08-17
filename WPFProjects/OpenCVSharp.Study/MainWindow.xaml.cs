using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using OpenCvSharp;

namespace OpenCVSharp.Study
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        /// <summary>
        /// OpenCvSharp 视频捕获对象
        /// </summary>
        private VideoCapture videoCapture;

        private Frame frame = new Frame();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            videoCapture = new VideoCapture();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            OpenFileDialog open = new OpenFileDialog()
            {
                Multiselect = false,
                Title = "请选择文件",
                Filter = "视频文件(*.mp4, *.wmv, *.mkv, *.flv)|*.mp4;*.wmv;*.mkv;*.flv|所有文件(*.*)|*.*"
            };

            if (open.ShowDialog() is true)
            {
                videoCapture.Open(open.FileName, VideoCaptureAPIs.ANY);

                Task.Run(() =>
                {
                    ShowMove();
                });

            }
        }


        private void ShowMove()
        {
            while (videoCapture.IsOpened())
            {
                Mat mat = new Mat();

                if (!videoCapture.Read(mat)) break;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = mat.ToMemoryStream();
                bitmapImage.EndInit();

                frame.ImageSource = bitmapImage;
            }
        }

        public class Frame : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private ImageSource imageSource;
            public ImageSource ImageSource
            {
                get { return imageSource; }
                set
                {
                    imageSource = value;
                    OnPropertyChanged();
                }
            }

            protected void OnPropertyChanged([CallerMemberName] string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
