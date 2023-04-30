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
        #region 基础属性、字段
        private readonly IntPtr hWnd = IntPtr.Zero;

        /// <summary>
        /// 播放进度
        /// </summary>
        private float VlcPosition { get; set; }

        /// <summary>
        /// 播放列表
        /// </summary>
        private string[] PlayList { get; set; }

        /// <summary>
        /// 播放序列
        /// </summary>
        private int PlayerIndex { get; set; } = 0;

        /// <summary>
        /// Title列表
        /// </summary>
        private string[] TitleList { get; set; }

        /// <summary>
        /// 是否循环播放
        /// </summary>
        private bool IsLoop { get; set; } = false;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Init();

            hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle(); // 获取当前窗口句柄
            if (WindowsCorner.OSVersion())
            {
                WindowCorner(WindowsCorner.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND);
            }
        }

        /// <summary>
        /// 播放器内核初始化
        /// </summary>
        private void Init()
        {
            Dispatcher.BeginInvoke(() =>
            {
                VLCMediaPlayer.MediaPlayer = new MediaPlayer(CommonClass.VLCMedia)
                {
                    Volume = 100

                };
                VLCMediaPlayer.MediaPlayer.Play();
            });
        }

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
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            BtnPlayOrStop_Click(null, null);
                        }));
                        break;

                    case Key.Enter: // 全屏调整
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            IsFullScreen();
                        }));
                        break;

                    case Key.Right: // 快进
                        Dispatcher.Invoke(() =>
                        {
                            BtnFastForward_Click(null, null);
                        });
                        break;

                    case Key.Left: // 快退
                        Dispatcher.Invoke(() =>
                        {
                            BtnBackOff_Click(null, null);
                        });
                        break;

                    case Key.Up: // 音量增
                        if (VLCMediaPlayer.MediaPlayer.Volume <= 100)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                VLCMediaPlayer.MediaPlayer.Volume += 5;
                            });
                        }
                        break;

                    case Key.Down: // 音量减
                        Dispatcher.Invoke(() =>
                        {
                            VLCMediaPlayer.MediaPlayer.Volume -= 5;
                        });
                        break;
                }
            }));
        }

        /// <summary>
        /// 窗体移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// 全屏调整
        /// </summary>
        private void IsFullScreen()
        {
            if (WindowState != WindowState.Maximized)
            {
                GdTitle.Visibility = GdBottom.Visibility = Visibility.Collapsed;
                WindowCorner(WindowsCorner.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND);
            }
            else
            {
                GdTitle.Visibility = GdBottom.Visibility = Visibility.Visible;
                WindowCorner(WindowsCorner.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND);
            }

            WindowState = WindowState is WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        /// <summary>
        /// 选择单文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                OpenFileDialog open = new()
                {
                    Filter = "视频文件(*.mp4, *.wmv, *.mkv, *.flv)|*.mp4;*.wmv;*.mkv;*.flv|所有文件(*.*)|*.*"
                };
                if (open.ShowDialog() is true)
                {
                    ImportPlayerList(open.FileNames, open.SafeFileNames);
                }
            }));
        }

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
                    ImportPlayerList(open.FileNames, open.SafeFileNames);
                }
            }));
        }

        /// <summary>
        /// 导入播放列表
        /// </summary>
        /// <param name="list"></param>
        private void ImportPlayerList(string[] filelist, string[] nameList)
        {
            PlayList = filelist;
            TitleList = nameList;
            PlayerIndex = 0;
            VLCMediaPlayer.MediaPlayer.Play(new Media(CommonClass.VLCMedia, PlayList[PlayerIndex], FromType.FromPath));
            LabelTitle.Content = string.Format("{0} - VLCMediaPlayer", TitleList[PlayerIndex]);
            VLCMediaPlayer.MediaPlayer.EndReached += MediaPlayer_EndReached;

            BtnPlayOrStop.Content = CommonClass.StringToUnicode("&#xe867;");
            BtnBackOff.Visibility = BtnFastForward.Visibility = Visibility.Visible;
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
                // 单文件定义是否循环播放
                if (PlayList.Length is 1 && IsLoop is true)
                {
                    PlayerIndex = ++PlayerIndex % PlayList.Length;
                    VLCMediaPlayer.MediaPlayer.Play(new Media(CommonClass.VLCMedia, PlayList[PlayerIndex], FromType.FromPath));
                    LabelTitle.Content = string.Format("{0} - VLCMediaPlayer", TitleList[PlayerIndex]);
                    return;
                }

                #region 多文件播放模式

                if (IsLoop)
                {
                    PlayerIndex = ++PlayerIndex % PlayList.Length;
                    VLCMediaPlayer.MediaPlayer.Play(new Media(CommonClass.VLCMedia, PlayList[PlayerIndex], FromType.FromPath));
                    LabelTitle.Content = string.Format("{0} - VLCMediaPlayer", TitleList[PlayerIndex]);
                    return;
                }

                PlayerIndex++;
                if (PlayerIndex < PlayList.Length)
                {
                    VLCMediaPlayer.MediaPlayer.Play(new Media(CommonClass.VLCMedia, PlayList[PlayerIndex], FromType.FromPath));
                    LabelTitle.Content = string.Format("{0} - VLCMediaPlayer", TitleList[PlayerIndex]);
                }

                #endregion
            }));
        }

        /// <summary>
        /// 是否开启循环播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoop_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (IsLoop)
                {
                    IsLoop = false;
                    BtnLoop.Content = "  开启循环播放  ";
                    return;
                }
                else
                {
                    IsLoop = true;
                    BtnLoop.Content = "  取消循环播放  ";
                }
            }));
        }

        /// <summary>
        /// 播放网络视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNetworkVideo_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                PlayList = null;
                NetworkUrlWindow networkUrlWindow = new();
                networkUrlWindow.ShowDialog();
                if (networkUrlWindow.DialogResult is true)
                {
                    VLCMediaPlayer.MediaPlayer.Stop();
                    // 测试地址：http://cctvalih5ca.v.myalicdn.com/live/cctv12/index.m3u8
                    //VLCMediaPlayer.MediaPlayer.NetworkCaching = CommonClass.CacheTime == 0 ? 100 : CommonClass.CacheTime;
                    using Media _media = new(CommonClass.VLCMedia, CommonClass.NetworkUrl, FromType.FromLocation);
                    _media.AddOption(":rtsp-tcp");
                    _media.AddOption(":clock-synchro=0"); // 时钟同步器
                    _media.AddOption(":live-caching=10"); // 实时缓存
                    _media.AddOption($":network-caching={CommonClass.CacheTime}"); // 网络缓存
                    _media.AddOption(":file-caching=0"); // 文件缓存
                    _media.AddOption(":grayscale"); // 灰度
                    VLCMediaPlayer.MediaPlayer.Play(_media);
                }
                BtnPlayOrStop.Content = CommonClass.StringToUnicode("&#xe867;");
                BtnBackOff.Visibility = BtnFastForward.Visibility = Visibility.Collapsed;
            });
        }

        /// <summary>
        /// 快退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBackOff_Click(object sender, RoutedEventArgs e)
        {
            VLCMediaPlayer.MediaPlayer.Time -= 5000;
        }

        /// <summary>
        /// 快进
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFastForward_Click(object sender, RoutedEventArgs e)
        {
            VLCMediaPlayer.MediaPlayer.Time += 5000;
        }

        /// <summary>
        /// 播放/暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlayOrStop_Click(object sender, RoutedEventArgs e)
        {
            if (VLCMediaPlayer.MediaPlayer.IsPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        private void Play()
        {
            VLCMediaPlayer.MediaPlayer.Play();
            BtnPlayOrStop.Content = CommonClass.StringToUnicode("&#xe867;");
        }

        /// <summary>
        /// 暂停
        /// </summary>
        private void Pause()
        {
            VLCMediaPlayer.MediaPlayer.Pause();
            BtnPlayOrStop.Content = CommonClass.StringToUnicode("&#xe864;");
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WindowCorner(WindowsCorner.DWM_WINDOW_CORNER_PREFERENCE preference)
        {
            WindowsCorner.DwmSetWindowAttribute(hWnd, WindowsCorner.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, sizeof(uint));
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;

            else e.Effects = DragDropEffects.None;
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            var list = (string[])e.Data.GetData(DataFormats.FileDrop);
            ImportPlayerList(list, new string[] { list[0].Split('\\')[^1] });
        }
    }
}
