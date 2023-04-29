using AForge.Video.DirectShow;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ZXing;

namespace HealthCodeIdentification
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private FilterInfoCollection _videoDevices;
        private VideoCaptureDevice _videoSource;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            OpenVideoCaptureDevice();
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_videoSource is null) return;

            _videoSource.SignalToStop();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// 查找并打开摄像头
        /// </summary>
        private void OpenVideoCaptureDevice()
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (_videoDevices.Count > 0)
            {
                _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
                _videoSource.NewFrame += _camera_NewFrame;
                _videoSource.Start();
                txtTips.Text = "AI扫描已启动，请将健康码对准摄像头";
                return;
            }

            MessageBox.Show("请连接摄像头！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// 加载视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private async void _camera_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            var image = eventArgs.Frame.Clone();
            Image videoImage = (Bitmap)image;
            Bitmap bitmapImage = (Bitmap)image;

            bitmapImage.RotateFlip(RotateFlipType.RotateNoneFlipX); // 设置图像旋转

            Graphics g = Graphics.FromImage(videoImage);

            //SolidBrush brush = new SolidBrush(Color.Red);
            //g.DrawString($"时间：{DateTime.Now:yyyy年MM月dd日 HH时mm分ss秒}", new Font("Arial", 18), brush, new PointF(5, 5));
            //brush.Dispose();
            //g.Dispose();

            MemoryStream ms = new MemoryStream();
            videoImage.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            bi.Freeze();

            await Dispatcher.BeginInvoke(new ThreadStart(async delegate
            {
                imageVideo.Source = bi;
                await QRCodeReader(bitmapImage);
            }));
        }

        private async Task<Task> QRCodeReader(Bitmap bitmap)
        {
            await Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                BarcodeReader barcodeReader = new BarcodeReader();
                var result = barcodeReader.Decode(bitmap);
                if (result != null)
                {
                    txtInfo.Text = result.ToString();
                    if (result.ToString().StartsWith("{"))
                    {
                        var data = JsonConvert.DeserializeObject<CodeData>(result.ToString());
                        switch (data.c.ToUpper())
                        {
                            case "G":
                                txtCodeColor.Text = "绿码";
                                break;
                            case "Y":
                                txtCodeColor.Text = "黄码";
                                break;
                            case "R":
                                txtCodeColor.Text = "红码";
                                break;
                        }
                    }
                }
            }));
            return Task.CompletedTask;
        }
    }

    public class CodeData
    {
        public string label { get; set; }
        public string cid { get; set; }
        public string cidtype { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string encode { get; set; }
        /// <summary>
        /// 颜色，绿码G，黄码Y，红码R
        /// </summary>
        public string c { get; set; }
        public string t { get; set; }
        public string v { get; set; }
        public string s { get; set; }
    }
}
