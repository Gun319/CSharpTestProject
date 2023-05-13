using Microsoft.Win32;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace QRCodeTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 4; i++)
            {
                CombECC.Items.Add(i);
            }
            CombECC.SelectedIndex = 2;

            string[] color = new string[] { "白", "黑", "红", "橙", "黄", "绿", "青", "蓝", "紫", "褐", "粉", "灰" };
            foreach (var item in color)
            {
                CombForegroundColor.Items.Add(item);
                CombBackgroundColor.Items.Add(item);
            }
            CombForegroundColor.SelectedIndex = 1;
            CombBackgroundColor.SelectedIndex = 0;
        }

        /// <summary>
        /// 水印图片路径
        /// </summary>
        public string? WatermarkPath { get; set; }

        private void BtnQRCode_Click(object sender, RoutedEventArgs e)
        {
            //生成二维码的内容
            string strCode = txtQrInfo.Text.Trim();
            QRCodeGenerator qrGenerator = new();

            // 容错率
            QRCodeGenerator.ECCLevel ECCLevel = (int)CombECC.SelectedItem == 0 ? QRCodeGenerator.ECCLevel.L : (int)CombECC.SelectedItem == 1 ? QRCodeGenerator.ECCLevel.M : (int)CombECC.SelectedItem == 2 ? QRCodeGenerator.ECCLevel.Q : QRCodeGenerator.ECCLevel.H;

            QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, ECCLevel); // L, M, Q, H 对应 0,1,2,3
            QRCode qrcode = new(qrCodeData);

            /* GetGraphic方法参数说明
                 public Bitmap GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, Bitmap icon = null, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true)
             * 
                 int pixelsPerModule:生成二维码图片的像素大小 
             * 
                 Color 二维码前景色   一般设置为Color.Black 黑色
             * 
                 Color 二维码背景色   一般设置为Color.White 白色
             * 
                 Bitmap icon :二维码 水印图标 例如：Bitmap icon = new Bitmap(context.Server.MapPath("~/images/zs.png")); 默认为NULL ，加上这个二维码中间会显示一个图标
             * 
                 int iconSizePercent： 水印图标的大小比例
             * 
                 int iconBorderWidth： 水印图标的边框
             * 
                 bool drawQuietZones:静止区，位于二维码某一边的空白边界,用来阻止读者获取与正在浏览的二维码无关的信息 即是否绘画二维码的空白边框区域 默认为true
            */

            Bitmap qrCodeImage;
            if (!string.IsNullOrWhiteSpace(WatermarkPath))
            {
                Bitmap icon = new(WatermarkPath);
                qrCodeImage = qrcode.GetGraphic(5, GetColor(CombForegroundColor.SelectedItem.ToString()), GetColor(CombBackgroundColor.SelectedItem.ToString()), icon, 25, 1);
            }
            else
            {
                qrCodeImage = qrcode.GetGraphic(5, GetColor(CombForegroundColor.SelectedItem.ToString()), GetColor(CombBackgroundColor.SelectedItem.ToString()), null, 25, 1);
            }

            Bitmap b = new(qrCodeImage);
            MemoryStream ms = new();
            b.Save(ms, ImageFormat.Png);
            byte[] bytes = ms.ToArray();
            ms.Close();

            //Convert it to BitmapImage
            BitmapImage image = new();
            image.BeginInit();
            image.StreamSource = new MemoryStream(bytes);
            image.EndInit();

            ImgQRCode.Source = image;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveImage();
        }

        private void BtnEmpty_Click(object sender, RoutedEventArgs e)
        {

            txtQrInfo.Text = string.Empty;
            ImgQRCode.Source = ImgWatermark.Source = null;
            CombECC.SelectedIndex = 2;
            CombForegroundColor.SelectedIndex = 1;
            CombBackgroundColor.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new()
            {
                Filter = "图片(*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png"
            };

            if (openFile.ShowDialog() is false) return;
            else
            {
                WatermarkPath = openFile.FileName;
                ImgWatermark.Source = new BitmapImage(new Uri(WatermarkPath, UriKind.RelativeOrAbsolute));
            }
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            var list = (string[])e.Data.GetData(DataFormats.FileDrop);
            WatermarkPath = list.FirstOrDefault();
            ImgWatermark.Source = new BitmapImage(new Uri(WatermarkPath, UriKind.RelativeOrAbsolute));
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;

            else e.Effects = DragDropEffects.None;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="saveImgPath"></param>
        public void SaveImage()
        {
            if (ImgQRCode.Source is null) return;

            SaveFileDialog dlg = new()
            {
                Title = "图片另存为",
                FileName = Guid.NewGuid().ToString(),
                DefaultExt = ".png",
                Filter = $"图像文件(*.png, *.bmp, *.jpg)|*.png;*.bmp;*.jpg|所有文件(*.*)|*.*"

            };

            if (dlg.ShowDialog() is false || !dlg.FileName.Contains('\\')) return;

            string saveImgPath = dlg.FileName;
            BitmapSource? BS = ImgQRCode.Source as BitmapSource;
            PngBitmapEncoder PBE = new();
            PBE.Frames.Add(BitmapFrame.Create(BS));
            using Stream stream = File.Create(saveImgPath);
            PBE.Save(stream);
        }

        /// <summary>
        /// 二维码颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color GetColor(string? color)
        {
            return color switch // "白", "黑", "红", "橙", "黄", "绿", "青", "蓝", "紫", "褐", "粉", "灰" 
            {
                "白" => Color.White,
                "黑" => Color.Black,
                "红" => Color.Red,
                "橙" => Color.Orange,
                "黄" => Color.Yellow,
                "绿" => Color.Green,
                "青" => Color.Cyan,
                "蓝" => Color.Blue,
                "紫" => Color.Purple,
                "褐" => Color.Brown,
                "粉" => Color.Pink,
                "灰" => Color.Gray,
                _ => Color.White,
            };
        }
    }
}
