using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenCvSharp;

namespace ImageProcessing.ViewModels.Business
{
    public partial class ImageSynthesisVideoViewModel : ViewModelBase
    {
        #region 通知属性、私有属性、命令、变量
        /// <summary>
        /// 文件格式
        /// </summary>
        [ObservableProperty]
        private string _fileSuffix = "jpg";

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
        /// 帧率
        /// </summary>
        [ObservableProperty]
        private string _fps = "0";

        /// <summary>
        /// Loading Visibility
        /// </summary>
        [ObservableProperty]
        private Visibility _loadingVisibility = Visibility.Collapsed;

        /// <summary>
        /// 视频预览
        /// </summary>
        [ObservableProperty]
        private string _videoPreview = string.Empty;

        private VideoWriter? _videoWriter;

        #endregion

        public ImageSynthesisVideoViewModel()
        {
            SourceMaterialList = [];

            BindingOperations.EnableCollectionSynchronization(SourceMaterialList, this);
        }

        #region Command Methods

        [RelayCommand]
        private Task Import()
        {
            OpenFolderDialog folderBrowser = new();

            if (folderBrowser.ShowDialog() is true)
                SourceMaterialPath = folderBrowser.FolderName;

            SourceMaterialListAddItem(LoadDir(SourceMaterialPath));

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task StartSynthesis()
        {
            //if (!FileChecked("发现您未导入素材，请先导入素材")) return Task.CompletedTask;
            if (SourceMaterialList.Count is 0)
            {
                MessageBox.Show("发现您未导入素材，请先导入素材", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return Task.CompletedTask;
            }

            SaveFileDialog? saveFile = new()
            {
                AddExtension = true,
                CheckPathExists = true,
                Filter = "avi视频|*.avi"
            };

            if (saveFile.ShowDialog() is true)
            {
                LoadingVisibility = Visibility.Visible;
                FinishProductSavePath = saveFile.FileName;

                VideoSynthesis();

                return Task.CompletedTask;
            }
            LoadingVisibility = Visibility.Collapsed;

            return Task.CompletedTask;
        }

        [RelayCommand]
        private static Task OpenImageLocation(TextBlock parameter)
        {
            if (parameter.Tag is not null)
                PositioningFile(parameter.Tag.ToString()!);

            return Task.CompletedTask;
        }

        [RelayCommand]
        private static Task OpenFinishProduct(TextBlock parameter)
        {
            if (!string.IsNullOrWhiteSpace(parameter.Text))
                PositioningFile(parameter.Text);

            return Task.CompletedTask;
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
            string videoFilePath = (parameter.Data.GetData(DataFormats.FileDrop) as Array)!.OfType<string>().ToList().FirstOrDefault()!;
            VideoPreview = videoFilePath;

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
                    VideoPreview = string.Empty;
                    VideoPreview = FinishProductSavePath;
                }
            });
        }

        /// <summary>
        /// 定位文件位置
        /// </summary>
        private static void PositioningFile(string filePath) => System.Diagnostics.Process.Start("explorer.exe", $"/select,{filePath}");
        #endregion

        #region 图片异步加载         
        private readonly object _lockObject = new();

        private void SourceMaterialListAddItem(List<SourceMaterials> strings)
        {
            Task.Run(() =>
            {
                foreach (var item in strings)
                    SourceMaterialList.Add(item);

                LoadingVisibility = Visibility.Collapsed;
            });
        }

        private List<SourceMaterials> LoadDir(string dirPath)
        {
            List<SourceMaterials> images = [];

            if (Directory.Exists(dirPath) && !string.IsNullOrWhiteSpace(FileSuffix))
            {
                images.AddRange(new DirectoryInfo(dirPath)
                    .GetFiles($"*.{FileSuffix}").OrderBy(o => o.FullName)
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
