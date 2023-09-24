using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageProcessing.Operates;
using Microsoft.Win32;
using OpenCvSharp;

namespace ImageProcessing.ViewModels
{
    public partial class BusinessControlViewModel : ViewModelBase
    {
        #region 通知属性、私有属性、命令、变量

        /// <summary>
        /// 素材路径
        /// </summary>
        [ObservableProperty]
        private string _sourceMaterialPath = string.Empty;

        /// <summary>
        /// 成品保存路径
        /// </summary>
        [ObservableProperty]
        private string _finishProductSavePath = string.Empty;

        /// <summary>
        /// 素材列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SourceMaterials> _sourceMaterialList;

        /// <summary>
        /// 成品列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<FinishProducts> _finishProductsList;

        /// <summary>
        /// 帧率
        /// </summary>
        [ObservableProperty]
        private double _fps;

        /// <summary>
        /// Loading Visibility
        /// </summary>
        [ObservableProperty]
        private Visibility _loadingVisibility = Visibility.Collapsed;

        /// <summary>
        /// 图像预览
        /// </summary>
        [ObservableProperty]
        private string _imagePreview = string.Empty;

        /// <summary>
        /// 视频预览
        /// </summary>
        [ObservableProperty]
        private string _videoPreview = string.Empty;

        /// <summary>
        /// 卡顿阈值
        /// </summary>
        [ObservableProperty]
        private int _stuckThreshold;

        private VideoWriter? _videoWriter;

        #endregion

        public BusinessControlViewModel()
        {
            SourceMaterialList = new ObservableCollection<SourceMaterials>();
            FinishProductsList = new ObservableCollection<FinishProducts>();

            BindingOperations.EnableCollectionSynchronization(SourceMaterialList, this);
            BindingOperations.EnableCollectionSynchronization(FinishProductsList, this);
        }

        #region Command Methods

        [RelayCommand]
        private Task Import()
        {
            OpenFileDialog(true);

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task StartSynthesis()
        {
            if (!FileChecked("发现您未导入素材，请先导入素材")) return Task.CompletedTask;

            SaveFileDialog saveFile = new()
            {
                AddExtension = true,
                CheckPathExists = true,
                Filter = "avi视频|*.avi"
            };

            if (saveFile.ShowDialog() is true)
            {
                LoadingVisibility = Visibility.Visible;
                SourceMaterialPath = saveFile.FileName;

                ConfigOperate.SetKeyValue(ConfigOperate.OpenFilePathKey, SourceMaterialPath);
                ConfigOperate.SetKeyValue(ConfigOperate.FPSKey, Fps.ToString());

                VideoSynthesis();
            }
            else
            {
                LoadingVisibility = Visibility.Collapsed;
            }

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task Preview(ListBox parameter)
        {
            if (parameter.SelectedItem is not null)
            {
                ImagePreview = (parameter.SelectedItem as SourceMaterials)!.FilePath!;
                parameter.SelectedIndex = -1;
            }
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task OpenImageLocation(TextBlock parameter)
        {
            if (parameter.Tag is not null)
                PositioningFile(parameter.Tag.ToString()!);

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task OpenFinishProduct(TextBlock parameter)
        {
            if (!string.IsNullOrWhiteSpace(parameter.Text))
                PositioningFile(parameter.Text);

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task DragEnter(DragEventArgs parameter)
        {
            parameter.Effects = parameter.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task Drop(DragEventArgs parameter)
        {
            string videoFilePath = (parameter.Data.GetData(DataFormats.FileDrop) as Array)!.OfType<string>().ToList().FirstOrDefault()!;
            VideoPreview = videoFilePath;
            FinishProductsListAddItem(videoFilePath);

            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 视频合成
        /// </summary>
        private void VideoSynthesis()
        {
            Mat mat = Cv2.ImRead(SourceMaterialList.FirstOrDefault()!.FilePath!);

            //int linLen = 100;

            //double x0 = mat.Width / 2;
            //double y0 = mat.Height / 2;

            //var top = new OpenCvSharp.Point(x0, y0 - linLen);
            //var bottom = new OpenCvSharp.Point(x0, y0 + linLen);

            //var left = new OpenCvSharp.Point(x0 - linLen, y0);
            //var right = new OpenCvSharp.Point(x0 + linLen, y0);

            Task.Run(() =>
            {
                if (!mat.Empty())
                {
                    _videoWriter = new VideoWriter(FinishProductSavePath, FourCC.XVID, Convert.ToDouble(Fps), new OpenCvSharp.Size(mat.Width, mat.Height));

                    foreach (var file in SourceMaterialList)
                    {
                        mat = Cv2.ImRead(file.FilePath!);
                        if (!mat.Empty())
                        {
                            //Cv2.Line(mat, top, bottom, Scalar.OrangeRed, 2);
                            //Cv2.Line(mat, left, right, Scalar.OrangeRed, 2);

                            _videoWriter.Write(mat);
                        }
                    }

                    _videoWriter.Release();
                    _videoWriter.Dispose();
                    mat.Dispose();

                    LoadingVisibility = Visibility.Collapsed;
                    VideoPreview = FinishProductSavePath;
                    ConfigOperate.SetKeyValue(ConfigOperate.SaveFilePathKey, FinishProductSavePath);
                    FinishProductsListAddItem(FinishProductSavePath);
                }
            });
        }

        readonly string defaultMessage = "您选择的文件夹中没有正确的素材（*.bmp），请检查您路径并重试";
        /// <summary>
        /// 检查素材
        /// </summary>
        /// <returns></returns>
        private bool FileChecked(string msg)
        {
            bool result = true;
            if (!strings.Any())
            {
                System.Windows.MessageBox.Show(msg, Process.GetCurrentProcess().MainWindowTitle, MessageBoxButton.OK);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 视频列表新增项
        /// </summary>
        /// <param name="videoFilePath"></param>
        private void FinishProductsListAddItem(string videoFilePath)
        {
            if (!FinishProductsList.Any(v => v.FilePath == videoFilePath))
            {
                FinishProductsList.Add(new FinishProducts
                {
                    FilePath = videoFilePath
                });
            }
        }

        /// <summary>
        /// 文件/文件夹选择对话框
        /// </summary>
        /// <param name="folderBrowse">是否文件夹浏览</param>
        /// <returns></returns>
        private static string OpenFileDialog(bool folderBrowse = false)
        {
            string path = string.Empty;
            Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog folderBrowser = new()
            {
                IsFolderPicker = folderBrowse
            };

            if (folderBrowser.ShowDialog() is Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
                path = folderBrowser.FileName;

            return path;
        }

        /// <summary>
        /// 定位文件位置
        /// </summary>
        private static void PositioningFile(string filePath) => System.Diagnostics.Process.Start("explorer.exe", $"/select,{filePath}");
        #endregion

        #region 图片异步加载

        List<SourceMaterials> strings = new();
        private readonly object _lockObject = new();

        private void ImageShowe(string path)
        {
            LoadingVisibility = Visibility.Visible;

            ConfigOperate.SetKeyValue(ConfigOperate.ThresholdKey, StuckThreshold.ToString());

            strings = LoadDir(path);
            SourceMaterialList.Clear();

            if (FileChecked(defaultMessage))
                SourceMaterialListAddItem();
        }

        private void SourceMaterialListAddItem()
        {
            Task.Run(() =>
            {
                lock (_lockObject)
                {
                    foreach (var item in strings)
                    {
                        Thread.Sleep(StuckThreshold);
                        SourceMaterialList.Add(item);
                    }
                }

                if (SourceMaterialList.Any())
                    ImagePreview = SourceMaterialList.FirstOrDefault()!.FilePath!;

                LoadingVisibility = Visibility.Collapsed;
            });
        }

        private static List<SourceMaterials> LoadDir(string dirPath)
        {
            List<SourceMaterials> images = new();

            if (Directory.Exists(dirPath))
            {
                images.AddRange(new DirectoryInfo(dirPath)
                    .GetFiles("*.bmp").OrderBy(o => o.FullName)
                    .Select(item => new SourceMaterials
                    {
                        FilePath = item.FullName
                    }));
            }
            return images;
        }

        #endregion
    }

    /// <summary>
    /// 素材
    /// </summary>
    public class SourceMaterials
    {
        public string? FilePath { get; set; }
    }

    /// <summary>
    /// 成品
    /// </summary>
    public class FinishProducts
    {
        public string? FilePath { get; set; }
    }
}
