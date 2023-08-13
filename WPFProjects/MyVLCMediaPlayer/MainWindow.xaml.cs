using LibVLCSharp.Shared;
using Microsoft.Win32;
using MyVLCMediaPlayer.ChildWindows;
using MyVLCMediaPlayer.Commons;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace MyVLCMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 基础属性、字段、构造/初始化函数
        private readonly IntPtr hWnd = IntPtr.Zero;

        /// <summary>
        /// 播放列表
        /// </summary>
        private List<string> PlayList { get; set; } = new List<string>(1);

        /// <summary>
        /// 播放序列
        /// </summary>
        private int PlayerIndex { get; set; } = 0;

        /// <summary>
        /// 是否循环播放
        /// </summary>
        private bool IsLoop { get; set; } = false;

        /// <summary>
        /// 是否改变进度条进度
        /// </summary>
        private bool IsValueChange { get; set; } = true;

        /// <summary>
        /// 媒体文件名称
        /// </summary>
        private string MediaMrl = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle(); // 获取当前窗口句柄

            Init();

            if (WindowsCorner.OSVersion())
                WindowCorner(WindowsCorner.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND);
        }

        /// <summary>
        /// 播放器内核初始化
        /// </summary>
        private void Init()
        {
            Task.Run(() =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    VLCMediaPlayer.MediaPlayer = new MediaPlayer(CommonClass.VLCMedia)
                    {
                        Volume = 100
                    };

                    VLCMediaPlayer.MediaPlayer.Playing += MediaPlayer_Playing; // 订阅播放开始事件
                    VLCMediaPlayer.MediaPlayer.Paused += MediaPlayer_Paused; // 订阅播放暂停事件
                    VLCMediaPlayer.MediaPlayer.Stopped += MediaPlayer_Stopped; // 订阅播放停止事件
                    VLCMediaPlayer.MediaPlayer.EndReached += MediaPlayer_EndReached; // 订阅播放结束事件
                    VLCMediaPlayer.MediaPlayer.PositionChanged += MediaPlayer_PositionChanged; // 订阅已播放时间事件
                    VLCMediaPlayer.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError; // 订阅播放错误事件
                    VLCMediaPlayer.MediaPlayer.MediaChanged += MediaPlayer_MediaChanged; // 订阅媒体改变事件
                    VLCMediaPlayer.MediaPlayer.Buffering += MediaPlayer_Buffering; // 订阅缓冲事件
                    VLCMediaPlayer.MediaPlayer.Corked += MediaPlayer_Corked; // 订阅播放器阻塞事件
                    VLCMediaPlayer.MediaPlayer.Uncorked += MediaPlayer_Uncorked; // 订阅播放器非阻塞事件
                }));
            });
        }

        #endregion

        #region VLC内核事件

        private void MediaPlayer_Playing(object? sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                BtnPlayOrStop.Content = CommonClass.StringToUnicode("&#xe867;");

                sd.Visibility = Visibility.Visible;
                sd.Minimum = 0;
                sd.Maximum = VLCMediaPlayer.MediaPlayer.Length;

                MediaLenght.Content = FormatTime(VLCMediaPlayer.MediaPlayer.Length);
            });
        }

        private void MediaPlayer_Paused(object? sender, EventArgs e) => BtnStopIcon();

        private void MediaPlayer_Stopped(object? sender, EventArgs e) => BtnStopIcon();

        private void MediaPlayer_PositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MediaPosition.Content = FormatTime(VLCMediaPlayer.MediaPlayer.Time);

                fps.Text = $"{VLCMediaPlayer.MediaPlayer.Fps} fps";

                if (IsValueChange)
                    sd.Value = VLCMediaPlayer.MediaPlayer.Time;
            });
        }

        private void MediaPlayer_EncounteredError(object? sender, EventArgs e)
        {
            MessageBox.Show("播放错误请检查媒体文件", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void MediaPlayer_MediaChanged(object? sender, MediaPlayerMediaChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MediaMrl = System.Web.HttpUtility.UrlDecode(VLCMediaPlayer.MediaPlayer.Media.Mrl).Split('/').Last();
                LabelTitle.Content = $"VLCMediaPlayer - {MediaMrl}";
            });
        }

        private void MediaPlayer_Buffering(object? sender, MediaPlayerBufferingEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void MediaPlayer_Corked(object? sender, EventArgs e)
        {
            MessageBox.Show("播放器已阻塞");
        }

        private void MediaPlayer_Uncorked(object? sender, EventArgs e)
        {
            MessageBox.Show("播放器已非阻塞");
        }

        #endregion

        #region 窗口事件

        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                switch (e.Key)
                {
                    case Key.Space:
                        Dispatcher.BeginInvoke(new Action(() => BtnPlayOrStop_Click(null, null)));
                        break;

                    case Key.Enter: // 全屏调整
                        Dispatcher.BeginInvoke(new Action(() => IsFullScreen()));
                        break;

                    case Key.Right: // 快进
                        Dispatcher.Invoke(() => BtnFastForward_Click(null, null));
                        break;

                    case Key.Left: // 快退
                        Dispatcher.Invoke(() => BtnBackOff_Click(null, null));
                        break;

                    case Key.Up: // 音量增
                        if (VLCMediaPlayer.MediaPlayer.Volume <= 100)
                            Dispatcher.Invoke(() => VLCMediaPlayer.MediaPlayer.Volume += 5);
                        break;

                    case Key.Down: // 音量减
                        Dispatcher.Invoke(() => VLCMediaPlayer.MediaPlayer.Volume -= 5);
                        break;
                }
            }));
        }

        /// <summary>
        /// 窗体移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        /// <summary>
        /// 选择多文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                OpenFileDialog open = new()
                {
                    Multiselect = true, // 是否可选择多个文件
                    Title = "请选择文件夹",
                    Filter = "视频文件(*.mp4, *.wmv, *.mkv, *.flv)|*.mp4;*.wmv;*.mkv;*.flv|所有文件(*.*)|*.*"
                };
                if (open.ShowDialog() is true)
                {
                    List<string> files = new();
                    files.AddRange(open.FileNames);
                    ImportPlayerList(files);
                }
            }));
        }

        /// <summary>
        /// 循环播放模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayer_EndReached(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //sd.Visibility = Visibility.Collapsed;

                fps.Text = "0 fps";

                if (PlayList.Any())
                {
                    // 单文件定义是否循环播放
                    if (IsLoop is true)
                    {
                        PlayerIndex = ++PlayerIndex % PlayList.Count;
                        VLCMediaPlayer.MediaPlayer.Play(new Media(CommonClass.VLCMedia, PlayList[PlayerIndex], FromType.FromPath));
                        return;
                    }

                    #region 多文件播放模式

                    if (IsLoop)
                    {
                        PlayerIndex = ++PlayerIndex % PlayList.Count;
                        VLCMediaPlayer.MediaPlayer.Play(new Media(CommonClass.VLCMedia, PlayList[PlayerIndex], FromType.FromPath));
                        return;
                    }

                    PlayerIndex++;

                    if (PlayerIndex < PlayList.Count)
                        VLCMediaPlayer.MediaPlayer.Play(new Media(CommonClass.VLCMedia, PlayList[PlayerIndex], FromType.FromPath));
                }
                #endregion
            }));
        }

        private void Loop_Checked(object sender, RoutedEventArgs e) => Dispatcher.BeginInvoke(() => { IsLoop = true; });

        private void Loop_Unchecked(object sender, RoutedEventArgs e) => Dispatcher.BeginInvoke(() => { IsLoop = false; });

        /// <summary>
        /// 播放网络视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNetworkVideo_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                PlayList = new List<string>(1);

                NetworkUrlWindow networkUrlWindow = new();
                networkUrlWindow.ShowDialog();
                if (networkUrlWindow.DialogResult is true)
                {
                    VLCMediaPlayer.MediaPlayer.Stop();
                    // 测试地址：http://cctvalih5ca.v.myalicdn.com/live/cctv12/index.m3u8
                    //VLCMediaPlayer.MediaPlayer.NetworkCaching = CommonClass.CacheTime == 0 ? 100 : CommonClass.CacheTime;
                    using Media _media = new(CommonClass.VLCMedia, CommonClass.NetworkUrl, FromType.FromLocation);
                    _media.AddOption(":rtsp-tcp");
                    _media.AddOption(":no-skip-frames");
                    _media.AddOption(":cr-average=10000");
                    _media.AddOption(":clock-synchro=0"); // 时钟同步器
                    _media.AddOption(":live-caching=10"); // 实时缓存
                    _media.AddOption($":network-caching={CommonClass.CacheTime}"); // 网络缓存
                    _media.AddOption(":file-caching=2000"); // 文件缓存
                    //_media.AddOption(":grayscale"); // 灰度             

                    VLCMediaPlayer.MediaPlayer.Play(_media);
                }
                //BtnBackOff.Visibility = BtnFastForward.Visibility = Visibility.Collapsed;
            });
        }

        /// <summary>
        /// 快退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBackOff_Click(object sender, RoutedEventArgs e) => VLCMediaPlayer.MediaPlayer.Time -= 5000;

        /// <summary>
        /// 快进
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFastForward_Click(object sender, RoutedEventArgs e) => VLCMediaPlayer.MediaPlayer.Time += 5000;

        /// <summary>
        /// 播放/暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlayOrStop_Click(object sender, RoutedEventArgs e)
        {
            if (VLCMediaPlayer.MediaPlayer.IsPlaying)
                VLCMediaPlayer.MediaPlayer.Pause();
            else
                VLCMediaPlayer.MediaPlayer.Play();
        }

        private void Fps_Checked(object sender, RoutedEventArgs e) => Dispatcher.BeginInvoke(() => fps.Visibility = Visibility.Visible);

        private void Fps_UnChecked(object sender, RoutedEventArgs e) => Dispatcher.BeginInvoke(() => fps.Visibility = Visibility.Collapsed);

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCloseApplication_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void BtnMinWindow_Click(object sender, RoutedEventArgs e) => Dispatcher.BeginInvoke(() => WindowState = WindowState.Minimized);

        private void Window_Drop(object sender, DragEventArgs e)
        {
            var list = (e.Data.GetData(DataFormats.FileDrop) as Array)!.OfType<string>().ToList();
            ImportPlayerList(list);
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;
        }

        private void Sd_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e) => IsValueChange = false;

        private void Sd_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                VLCMediaPlayer.MediaPlayer.Time = (long)sd.Value;
                IsValueChange = true;
            }));
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 导入播放列表
        /// </summary>
        /// <param name="list"></param>
        private void ImportPlayerList(List<string> filelist)
        {
            PlayList = filelist;
            PlayerIndex = 0;
            VLCMediaPlayer.MediaPlayer.Play(new Media(CommonClass.VLCMedia, PlayList[PlayerIndex], FromType.FromPath));

            BtnBackOff.Visibility = BtnFastForward.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 全屏调整
        /// </summary>
        private void IsFullScreen()
        {
            if (WindowState != WindowState.Maximized)
            {
                sd.Visibility = GdTitle.Visibility = GdBottom.Visibility = Visibility.Collapsed;
                WindowCorner(WindowsCorner.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND);
            }
            else
            {
                if (VLCMediaPlayer.MediaPlayer.IsPlaying)
                    sd.Visibility = Visibility.Visible;

                GdTitle.Visibility = GdBottom.Visibility = Visibility.Visible;
                WindowCorner(WindowsCorner.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND);
            }

            WindowState = WindowState is WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void BtnStopIcon() => Dispatcher.BeginInvoke(() => BtnPlayOrStop.Content = CommonClass.StringToUnicode("&#xe864;"));

        private void WindowCorner(WindowsCorner.DWM_WINDOW_CORNER_PREFERENCE preference)
        {
            WindowsCorner.DwmSetWindowAttribute(hWnd, WindowsCorner.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, sizeof(uint));
        }

        /// <summary>
        /// 时间格式化
        /// </summary>
        /// <param name="seconds">传入的毫秒</param>
        /// <returns></returns>
        private static string FormatTime(long seconds)
        {
            float time = seconds / 1000f;
            float hour = MathF.Floor(time / 3600f);
            float minute = MathF.Floor(time / 60f - hour * 60f);
            float second = time - minute * 60f - hour * 3600f;

            return $"{hour:00}:{minute:00}:{second:00}";
        }

        #endregion
    }
}
