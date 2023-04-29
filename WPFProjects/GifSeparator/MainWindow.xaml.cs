using GifSeparator.Common;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WindowCornerModel;

namespace GifSeparator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IntPtr hWnd = IntPtr.Zero;
        private BitmapDecoder bd;
        public MainWindow()
        {
            InitializeComponent();

            hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle(); // 获取当前窗口句柄
            if (WindowsCorner.OSVersion())
            {
                WindowsCorner.DWM_WINDOW_CORNER_PREFERENCE preference = WindowsCorner.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
                WindowsCorner.DwmSetWindowAttribute(hWnd, WindowsCorner.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, sizeof(uint));
            }
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

        private void ShowGifInfo(byte[] buffer, string imgPath)
        {
            //string type = Encoding.ASCII.GetString(buffer, 0, 3); //前3个字节，标识"GIF"
            string version = Encoding.ASCII.GetString(buffer, 3, 3);//3~6个字节，版本号
            int logicalWidth = BitConverter.ToUInt16(buffer, 6);//第7，8两个字节，宽度
            int logicalHeight = BitConverter.ToUInt16(buffer, 8); //第9，10两个字节，高度

            spInfo.Visibility = Visibility.Visible;
            spError.Visibility = Visibility.Collapsed;

            txtFileName.Text = string.Format("图片路径：{0}", imgPath);
            txtTotalFrames.Text = string.Format("总帧数：{0}", bd.Frames.Count.ToString());
            txtRealHeight.Text = string.Format("实际高度：{0}px", logicalHeight.ToString());
            txtRealWidth.Text = string.Format("实际宽度：{0}px", logicalWidth.ToString());
            txtVersion.Text = string.Format("Gif版本：{0}", version);
            txtErrorPrompt.Text = "";
        }

        public void ShowGif(string _imgPath)
        {
            FileStream fsImg = new(_imgPath, FileMode.Open, FileAccess.Read);

            MemoryStream gif = new();
            fsImg.CopyTo(gif);
            fsImg.Close();

            byte[] buffer = new byte[gif.Length];
            gif.Position = 0;
            gif.Read(buffer, 0, Convert.ToInt32(gif.Length));

            spGifImage.Children.Clear();
            GifImage gifImage = new()
            {
                Source = _imgPath
            };

            //判断图片是否为GIF格式，通过图片的前三个字节标识位来判断
            if (!Encoding.ASCII.GetString(buffer, 0, 3).ToLower().Equals("gif", StringComparison.Ordinal))
            {
                spInfo.Visibility = Visibility.Collapsed;
                spError.Visibility = Visibility.Visible;
                txtVersion.Text = txtRealWidth.Text = txtRealHeight.Text = txtTotalFrames.Text = txtFileName.Text = "";
                txtErrorPrompt.Text = "无效的GIF图片";

                //清空容器
                spFrames.Children.Clear();
                spGifImage.Children.Add(gifImage);

                return;
            }
            //清空容器
            spFrames.Children.Clear();
            spGifImage.Children.Clear();

            bd = new GifBitmapDecoder(gif, BitmapCreateOptions.None, BitmapCacheOption.Default);
            ShowGifInfo(buffer, _imgPath); //显示图片的信息
                                           //显示图片的所有帧
            for (int i = 0; i < bd.Frames.Count; i++)
            {
                Image img = new()
                {
                    Source = bd.Frames[i],
                    Width = 200,
                    Height = 200,
                    Margin = new Thickness(6, 0, 0, 0)
                };
                spFrames.Children.Add(img);
            }
            spGifImage.Children.Add(gifImage);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new();
            if (open.ShowDialog() == true)
            {
                ShowGif(open.FileName);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (bd != null)
            {
                SaveFileDialog save = new();
                if (save.ShowDialog() == true)
                {
                    for (int i = 0; i < bd.Frames.Count; i++)
                    {
                        FileStream stream = new(save.FileName + i + ".png", FileMode.Create);
                        PngBitmapEncoder encoder = new();
                        encoder.Frames.Add(bd.Frames[i]);
                        encoder.Save(stream);
                        stream.Close();
                    }

                    txtErrorPrompt.Text = "保存成功！";
                    spError.Visibility = Visibility.Visible;

                    //清空容器
                    //spFrames.Children.Clear();
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            txtVersion.Text = txtRealWidth.Text = txtRealHeight.Text = txtTotalFrames.Text = txtFileName.Text = txtErrorPrompt.Text = "";
            bd = null;
            //清空容器
            spGifImage.Children.Clear();
            spFrames.Children.Clear();
        }

        private void WindowClear_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
