using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace ImageProcessing.ViewModels.Business
{
    public partial class ComicViewModel : ViewModelBase
    {
        /// <summary>
        /// 图像
        /// </summary>
        [ObservableProperty]
        private BitmapSource? _comicImage;

        /// <summary>
        /// 模板图像
        /// </summary>
        [ObservableProperty]
        private string? _styleImage;

        public ComicViewModel()
        {
            StyleImage = $"{System.AppDomain.CurrentDomain.BaseDirectory}/Images/ic_300ys.png";
        }

        [RelayCommand]
        private static Task DragEnter(DragEventArgs parameter)
        {
            parameter.Effects = parameter.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task Drop(DragEventArgs parameter)
        {
            string filePath = (parameter.Data.GetData(DataFormats.FileDrop) as Array)!.OfType<string>().ToList().FirstOrDefault()!;

            if (!string.IsNullOrWhiteSpace(filePath))
                ComicImage = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(new Mat(filePath));

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task Build()
        {
            if (ComicImage is null || string.IsNullOrWhiteSpace(StyleImage))
            {
                MessageBox.Show($"{(ComicImage is null ? "请导入待处理图像" : string.IsNullOrWhiteSpace(StyleImage) ? "请导入模板图像" : string.Empty)}", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                return Task.CompletedTask;
            }

            // 加载输入图像
            Mat inputImage = new();
            Cv2.CopyTo(OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(ComicImage), inputImage);

            // 将输入图像转换为灰度图像
            Mat grayImage = new();
            Cv2.CvtColor(inputImage, grayImage, ColorConversionCodes.BGR2GRAY);
            // 对灰度图像进行阈值处理
            Mat thresholdImage = new();
            Cv2.Threshold(grayImage, thresholdImage, 100, 255, ThresholdTypes.Binary);
            // 对阈值图像进行中值滤波
            Mat blurredImage = new();
            Cv2.MedianBlur(thresholdImage, blurredImage, 5);
            // 对模糊图像进行边缘检测
            Mat edgeImage = new();
            Cv2.Canny(blurredImage, edgeImage, 100, 200);

            // 加载风格图像
            Mat styleImage = new(StyleImage);
            // 将风格图像转换为灰度图像
            Mat styleGrayImage = new();
            Cv2.CvtColor(styleImage, styleGrayImage, ColorConversionCodes.BGR2GRAY);
            // 对风格灰度图像进行阈值处理
            Mat styleThresholdImage = new();
            Cv2.Threshold(styleGrayImage, styleThresholdImage, 100, 255, ThresholdTypes.Binary);
            // 对风格阈值图像进行中值滤波
            Mat styleBlurredImage = new();
            Cv2.MedianBlur(styleThresholdImage, styleBlurredImage, 5);
            // 对风格模糊图像进行边缘检测
            Mat styleEdgeImage = new();
            Cv2.Canny(styleBlurredImage, styleEdgeImage, 100, 200);
            // 将风格边缘图像调整至与输入图像相同的大小
            Mat resizedStyleEdgeImage = new();
            Cv2.Resize(styleEdgeImage, resizedStyleEdgeImage, inputImage.Size());

            // 创建一个矩阵来存储输出图像
            Mat outputImage = new(inputImage.Rows, inputImage.Cols, MatType.CV_32FC1, new Scalar(0));
            // 遍历输入图像的每个像素
            for (int y = 0; y < inputImage.Rows; y++)
            {
                for (int x = 0; x < inputImage.Cols; x++)
                {
                    // 获取像素在输出图像中的坐标
                    int outputY = y;
                    int outputX = x;
                    // 检查像素坐标是否在输出图像的范围内
                    if (outputY >= 0 && outputY < outputImage.Rows && outputX >= 0 && outputX < outputImage.Cols)
                    {
                        // 计算像素在输入图像中的梯度大小和方向
                        double gradientMagnitude = CalculateGradientMagnitude(grayImage, x, y);
                        double gradientDirection = CalculateGradientDirection(grayImage, x, y);
                        // 根据梯度大小和方向，以及风格图像的像素值，计算输出图像的像素值
                        float outputPixelValue = CalculateOutputPixelValue(gradientMagnitude, gradientDirection, resizedStyleEdgeImage.At<byte>(x, y));
                        // 在输出图像中设置像素值
                        outputImage.At<byte>(outputY, outputX) = (byte)outputPixelValue;
                    }
                }
            }

            ComicImage = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(outputImage);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 计算梯度大小函数
        /// </summary>
        /// <param name="grayImage"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static double CalculateGradientMagnitude(Mat grayImage, int x, int y)
        {
            double gradientMagnitude = 0;
            // 使用Sobel算子计算梯度大小
            Mat imageGradientX = new();
            Mat imageGradientY = new();
            Cv2.Sobel(grayImage, imageGradientX, MatType.CV_32FC1, 1, 0, 3, 1);
            Cv2.Sobel(grayImage, imageGradientY, MatType.CV_32FC1, 0, 1, 3, 1);
            gradientMagnitude = Math.Sqrt(imageGradientX.At<float>(y, x) * imageGradientX.At<float>(y, x) + imageGradientY.At<float>(y, x) * imageGradientY.At<float>(y, x)) / 255.0;
            return gradientMagnitude;
        }

        /// <summary>
        /// 计算梯度方向函数
        /// </summary>
        /// <param name="grayImage"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static double CalculateGradientDirection(Mat grayImage, int x, int y)
        {
            // 使用Sobel算子计算梯度方向
            Mat imageGradientX = new();
            Mat imageGradientY = new();
            Cv2.Sobel(grayImage, imageGradientX, MatType.CV_32FC1, 1, 0, 3, 1);
            Cv2.Sobel(grayImage, imageGradientY, MatType.CV_32FC1, 0, 1, 3, 1);
            double gradientDirection = Math.Atan2(imageGradientY.At<float>(y, x), imageGradientX.At<float>(y, x)) * 180.0 / Math.PI;
            return gradientDirection;
        }

        /// <summary>
        /// 根据梯度大小、梯度方向和风格图像的像素值计算输出图像的像素值函数
        /// </summary>
        /// <param name="gradientMagnitude"></param>
        /// <param name="gradientDirection"></param>
        /// <param name="stylePixelValue"></param>
        /// <returns></returns>
        private static byte CalculateOutputPixelValue(double gradientMagnitude, double gradientDirection, byte stylePixelValue)
        {
            // 根据梯度大小、梯度方向和风格图像的像素值计算输出图像的像素值
            byte outputPixelValue = (byte)(gradientMagnitude * gradientDirection + stylePixelValue);
            return outputPixelValue;
        }
    }
}
