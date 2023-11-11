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
    public partial class MatchTemplateViewModel : ViewModelBase
    {
        /// <summary>
        /// 被匹配图像
        /// </summary>
        [ObservableProperty]
        public string? _srcImage;

        /// <summary>
        /// 模板图像
        /// </summary>
        [ObservableProperty]
        public string? _tempImage;

        /// <summary>
        /// 匹配结果
        /// </summary>
        [ObservableProperty]
        public BitmapSource? matchingResults;

        /// <summary>
        /// 坐标
        /// </summary>
        [ObservableProperty]
        public string? _point;

        /// <summary>
        /// 匹配度
        /// </summary>
        [ObservableProperty]
        public string? _matchingDegree;

        public MatchTemplateViewModel() { }

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

            string elementName = (parameter.Source as FrameworkElement)!.Name;

            if (elementName is "srcImg")
                SrcImage = filePath;
            else if (elementName is "tempImg")
                TempImage = filePath;

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task Start()
        {
            if (SrcImage is not null && TempImage is not null)
            {
                var point = TemplateMatch(SrcImage, TempImage, out double matchVal);

                Point = $"X:{point.X} Y:{point.Y}";
                MatchingDegree = matchVal.ToString();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 模板匹配
        /// </summary>
        /// <param name="srcImg">被匹配图像路径</param>
        /// <param name="tempImg">模板图像路径</param>
        /// <param name="matchVal"></param>
        /// <returns>当前匹配完成后 模板在匹配图像中左上角的坐标点</returns>
        public OpenCvSharp.Point TemplateMatch(string srcImg, string tempImg, out double matchVal)
        {
            OpenCvSharp.Point tempPoint = new();
            matchVal = 0d;

            Mat srcImage = Cv2.ImRead(srcImg, ImreadModes.Color);
            Mat tempImage = Cv2.ImRead(tempImg, ImreadModes.Color);

            if (srcImage.Width > tempImage.Width && srcImage.Height > tempImage.Height)
                MatchingResults = MatchTemplate(tempImage, srcImage, out tempPoint, out matchVal);

            else
                MessageBox.Show("模板图像不可以大于被匹配图像", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

            return tempPoint;
        }

        /// <summary>
        /// 模板匹配
        /// </summary>
        /// <param name="tempalte">模板图片像</param>
        /// <param name="srcPic">被匹配图像</param>
        /// <param name="tempPoint">返回模板图像在匹配图像中的坐标位置</param>
        /// <param name="matchValue"></param>
        /// <returns>标注匹配区域的图像</returns>
        private static BitmapSource MatchTemplate(Mat tempalte, Mat srcPic, out OpenCvSharp.Point tempPoint, out double matchValue)
        {
            Mat result = new();

            //模板匹配
            Cv2.MatchTemplate(srcPic, tempalte, result, TemplateMatchModes.CCoeffNormed);// CCoeffNormed 最好匹配为1,值越小匹配越差
            double minVal;
            double maxVal;

            OpenCvSharp.Point minLoc = new(0, 0);
            OpenCvSharp.Point maxLoc = new(0, 0);
            OpenCvSharp.Point matchLoc = new(0, 0);

            // 归一化
            Cv2.Normalize(result, result, 0, 1, NormTypes.MinMax, -1);

            //寻找极值
            Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);

            // 最大值坐标
            matchLoc = maxLoc;

            // 复制整个矩阵
            Mat mask = srcPic.Clone();

            // 画框显示 :对角线画框，起点和终点，都用Point，线宽
            Cv2.Rectangle(mask, matchLoc, new OpenCvSharp.Point(matchLoc.X + tempalte.Cols, matchLoc.Y + tempalte.Rows), Scalar.Green, 2);// 2 代表画的线条的宽细程度

            //Console.WriteLine("最大值：{0}，X:{1}，Y:{2}", maxVal, matchLoc.Y, matchLoc.X);
            matchValue = maxVal;
            tempPoint.X = matchLoc.X;
            tempPoint.Y = matchLoc.Y;

            //循环查找画框显示
            double threshold = 0.91;

            Mat maskMulti = srcPic.Clone();
            for (int i = 1; i < result.Rows - tempalte.Rows; i += tempalte.Rows)
            {
                for (int j = 1; j < result.Cols - tempalte.Cols; j += tempalte.Cols)
                {
                    OpenCvSharp.Rect roi = new(j, i, tempalte.Cols, tempalte.Rows); // 建立感兴趣
                    Mat RoiResult = new(result, roi);
                    Cv2.MinMaxLoc(RoiResult, out minVal, out maxVal, out minLoc, out maxLoc);// 查找极值
                    matchLoc = maxLoc;//最大值坐标
                    if (maxVal > threshold)
                    {
                        //画框显示
                        Cv2.Rectangle(maskMulti, new OpenCvSharp.Point(j + maxLoc.X, i + maxLoc.Y), new OpenCvSharp.Point(j + maxLoc.X + tempalte.Cols, i + maxLoc.Y + tempalte.Rows), Scalar.Green, 2);

                        string axis = '(' + Convert.ToString(i + maxLoc.Y) + ',' + Convert.ToString(j + maxLoc.X) + ')';
                        Cv2.PutText(maskMulti, axis, new OpenCvSharp.Point(j + maxLoc.X, i + maxLoc.Y), HersheyFonts.HersheyPlain, 1, Scalar.Red, 1, LineTypes.Link4);
                    }
                }
            }
            //返回匹配后图像
            return OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(mask);
        }
    }
}
