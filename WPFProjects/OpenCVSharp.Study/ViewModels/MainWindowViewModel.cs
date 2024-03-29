﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using NAudio.Wave;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Prism.Commands;

namespace OpenCVSharp.Study.ViewModels
{
    public class MainWindowViewModel : Prism.Mvvm.BindableBase
    {
        #region 属性、变量、命令

        private WriteableBitmap _bitmap;
        /// <summary>
        /// UI绑定的资源对象
        /// </summary>                
        public WriteableBitmap Bitmap
        {
            get => _bitmap;

            set => SetProperty(ref _bitmap, value);
        }

        /// <summary>
        /// OpenCvSharp 视频捕获对象
        /// </summary>
        private static VideoCapture videoCapture;

        private static Mat frame = new Mat();

        private static BitmapData bitmapData = new BitmapData();

        private static Bitmap bitmap;

        Int32Rect rect;

        static int width = 0, height = 0;

        /// <summary>
        /// 打开文件
        /// </summary>
        public DelegateCommand OpenFileCommand { get; set; }

        public DelegateCommand MNCommand { get; set; }

        #endregion

        public MainWindowViewModel()
        {
            videoCapture = new VideoCapture();

            OpenFileCommand = new DelegateCommand(OpenFile);
            MNCommand = new DelegateCommand(MN);
        }

        #region 私有方法

        private void OpenFile()
        {
            OpenFileDialog open = new OpenFileDialog()
            {
                Multiselect = false,
                Title = "请选择文件",
                Filter = "视频文件(*.mp4, *.wmv, *.mkv, *.flv)|*.mp4;*.wmv;*.mkv;*.flv|所有文件(*.*)|*.*"
            };

            if (open.ShowDialog() is true)
            {
                Task.Run(() =>
                {
                    NAudioPlay(open.FileName);
                });

                ShowMove(open.FileName);
            }
        }

        /// <summary>
        /// 获取视频
        /// </summary>
        /// <param name="fileName">文件路径</param>
        private void ShowMove(string fileName)
        {
            videoCapture.Open(fileName, VideoCaptureAPIs.ANY);

            if (videoCapture.IsOpened())
            {
                var timer = (int)Math.Round(1000 / videoCapture.Fps) - 8;
                width = videoCapture.FrameWidth;
                height = videoCapture.FrameHeight;

                Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                rect = new Int32Rect(0, 0, Bitmap.PixelWidth, Bitmap.PixelHeight);

                while (true)
                {
                    videoCapture.Read(frame);
                    if (!frame.Empty())
                    {
                        ShowImage();
                        Cv2.WaitKey(timer);
                    }
                }
            }
        }

        private void ShowImage()
        {
            Bitmap.Lock();

            bitmap = frame.ToBitmap();

            bitmapData = bitmap.LockBits(new Rectangle(new System.Drawing.Point(0, 0), bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            Bitmap.WritePixels(rect, bitmapData.Scan0, bitmapData.Height * bitmapData.Stride, bitmapData.Stride, 0, 0);

            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();

            Bitmap.Unlock();
        }

        /// <summary>
        /// 获取音频
        /// </summary>
        /// <param name="fileName">文件路径</param>
        private void NAudioPlay(string fileName)
        {
            using (var audioFile = new AudioFileReader(fileName))
            using (var outputDevice = new WaveOutEvent())
            {
                // 帧持续时间     采样数        采样率
                // FrameTime = SampleSize / SampleRate * 1000
                var frameTime = audioFile.WaveFormat.AverageBytesPerSecond / audioFile.WaveFormat.SampleRate * 1000;

                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState is PlaybackState.Playing)
                {
                    Thread.Sleep(frameTime);
                }
            }
        }

        #endregion

        private void MN()
        {
            int height = 800;
            int width = 800;
            // Create the bitmap, with the dimensions of the image placeholder.
            WriteableBitmap wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            // Define the update square (which is as big as the entire image).
            Int32Rect rect = new Int32Rect(0, 0, width, height);

            byte[] pixels = new byte[width * height * wb.Format.BitsPerPixel / 8];
            Random rand = new Random();
            for (int y = 0; y < wb.PixelHeight; y++)
            {
                for (int x = 0; x < wb.PixelWidth; x++)
                {
                    int alpha = 0;
                    int red = 0;
                    int green = 0;
                    int blue = 0;

                    // Determine the pixel's color.
                    if ((x % 5 == 0) || (y % 7 == 0))
                    {
                        red = (int)((double)y / wb.PixelHeight * 255);
                        green = rand.Next(100, 255);
                        blue = (int)((double)x / wb.PixelWidth * 255);
                        alpha = 255;
                    }
                    else
                    {
                        red = (int)((double)x / wb.PixelWidth * 255);
                        green = rand.Next(100, 255);
                        blue = (int)((double)y / wb.PixelHeight * 255);
                        alpha = 50;
                    }

                    int pixelOffset = (x + y * wb.PixelWidth) * wb.Format.BitsPerPixel / 8;
                    pixels[pixelOffset] = (byte)blue;
                    pixels[pixelOffset + 1] = (byte)green;
                    pixels[pixelOffset + 2] = (byte)red;
                    pixels[pixelOffset + 3] = (byte)alpha;


                }

                int stride = (wb.PixelWidth * wb.Format.BitsPerPixel) / 8;

                wb.WritePixels(rect, pixels, stride, 0);
            }

            // Show the bitmap in an Image element.
            Bitmap = wb;
        }
    }
}