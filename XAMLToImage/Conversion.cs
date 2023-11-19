using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XAMLToImage
{
    public class Conversion
    {
        public static void GenerateImage(Control control, string imageType, string path)
        {
            using FileStream stream = new(path, FileMode.OpenOrCreate);
            GenerateImage(control, imageType, stream);

            stream.Flush();
            stream.Close();
        }

        public static void GenerateImage(Control control, string imageType, Stream result)
        {
            if (control == null) return;

            control.InvalidateArrange();
            control.UpdateLayout();
            control.Background = Brushes.White;

            Size controlSize = RetrieveDesiredSize(control);
            Rect rect = new(0, 0, controlSize.Width, controlSize.Height);
            RenderTargetBitmap bitmapRender = new((int)controlSize.Width, (int)controlSize.Height, 96, 96, PixelFormats.Pbgra32);

            control.Arrange(rect);
            bitmapRender.Render(control);

            BitmapEncoder? encoder = null;
            //选取编码器
            switch (imageType.ToUpper())
            {
                case "BMP":
                    encoder = new BmpBitmapEncoder();
                    break;
                case "GIF":
                    encoder = new GifBitmapEncoder();
                    break;
                case "JPEG":
                    encoder = new JpegBitmapEncoder();
                    break;
                case "PNG":
                    encoder = new PngBitmapEncoder();
                    break;
                case "TIFF":
                    encoder = new TiffBitmapEncoder();
                    break;
                default:
                    break;
            }

            encoder?.Frames.Add(BitmapFrame.Create(bitmapRender));
            encoder?.Save(result);
        }

        private static Size RetrieveDesiredSize(Control control)
        {
            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return control.DesiredSize;
        }
    }
}
