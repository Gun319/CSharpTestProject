using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Threading;

namespace GifSeparator.Common
{
    public class GifImage : UserControl
    {
        private GifAnimation gifAnimation;

        private Image image;

        public GifImage()
        {

        }

        public static readonly DependencyProperty ForceGifAnimProperty = DependencyProperty.Register(nameof(ForceGifAnim), typeof(bool), typeof(GifImage), new FrameworkPropertyMetadata(false));
        public bool ForceGifAnim
        {
            get => (bool)GetValue(ForceGifAnimProperty);
            set => SetValue(ForceGifAnimProperty, value);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(string), typeof(GifImage), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnSourceChanged)));
        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GifImage obj = (GifImage)d;
            string s = (string)e.NewValue;
            obj.CreateFromSourceString(s);
        }

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(GifImage), new FrameworkPropertyMetadata(Stretch.Fill, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStretchChanged)));
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GifImage obj = (GifImage)d;
            Stretch s = (Stretch)e.NewValue;
            if (obj.gifAnimation != null)
            {
                obj.gifAnimation.Stretch = s;
            }
            else if (obj.image != null)
            {
                obj.image.Stretch = s;
            }
        }

        public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register(nameof(StretchDirection), typeof(StretchDirection), typeof(GifImage), new FrameworkPropertyMetadata(StretchDirection.Both, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStretchDirectionChanged)));
        public StretchDirection StretchDirection
        {
            get => (StretchDirection)GetValue(StretchDirectionProperty);
            set => SetValue(StretchDirectionProperty, value);
        }
        private static void OnStretchDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GifImage obj = (GifImage)d;
            StretchDirection s = (StretchDirection)e.NewValue;
            if (obj.gifAnimation != null)
            {
                obj.gifAnimation.StretchDirection = s;
            }
            else if (obj.image != null)
            {
                obj.image.StretchDirection = s;
            }
        }


        public delegate void ExceptionRoutedEventHandler(object sender, GifImageExceptionRoutedEventArgs args);

        public static readonly RoutedEvent ImageFailedEvent = EventManager.RegisterRoutedEvent(nameof(ImageFailed), RoutingStrategy.Bubble, typeof(ExceptionRoutedEventHandler), typeof(GifImage));

        public event ExceptionRoutedEventHandler ImageFailed
        {
            add
            {
                AddHandler(ImageFailedEvent, value);
            }
            remove
            {
                RemoveHandler(ImageFailedEvent, value);
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            RaiseImageFailedEvent(e.ErrorException);
        }

        private void RaiseImageFailedEvent(Exception exp)
        {
            GifImageExceptionRoutedEventArgs newArgs = new(ImageFailedEvent, this)
            {
                ErrorException = exp
            };
            RaiseEvent(newArgs);
        }

        private void DeletePreviousImage()
        {
            if (image != null)
            {
                RemoveLogicalChild(image);
                image = null;
            }
            if (gifAnimation != null)
            {
                RemoveLogicalChild(gifAnimation);
                gifAnimation = null;
            }
        }

        private void CreateNonGifAnimationImage()
        {
            image = new Image();
            image.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(Image_ImageFailed);
            ImageSource src = (ImageSource)new ImageSourceConverter().ConvertFromString(Source);
            image.Source = src;
            image.Stretch = Stretch;
            image.StretchDirection = StretchDirection;
            AddChild(image);
        }

        private void CreateGifAnimation(MemoryStream memoryStream)
        {
            gifAnimation = new GifAnimation();
            gifAnimation.CreateGifAnimation(memoryStream);
            gifAnimation.Stretch = Stretch;
            gifAnimation.StretchDirection = StretchDirection;
            AddChild(gifAnimation);
        }

        private void CreateFromSourceString(string source)
        {
            DeletePreviousImage();
            Uri uri;

            try
            {
                uri = new Uri(source, UriKind.RelativeOrAbsolute);
            }
            catch (Exception exp)
            {
                RaiseImageFailedEvent(exp);
                return;
            }

            if (source.Trim().ToUpper().EndsWith(".GIF") || ForceGifAnim)
            {
                if (!uri.IsAbsoluteUri)
                {
                    GetGifStreamFromPack(uri);
                }
                else
                {
                    string leftPart = uri.GetLeftPart(UriPartial.Scheme);

                    if (leftPart is "http://" or "ftp://" or "file://")
                    {
                        GetGifStreamFromHttp(uri);
                    }
                    else if (leftPart == "pack://")
                    {
                        GetGifStreamFromPack(uri);
                    }
                    else
                    {
                        CreateNonGifAnimationImage();
                    }
                }
            }
            else
            {
                CreateNonGifAnimationImage();
            }
        }

        private delegate void WebRequestFinishedDelegate(MemoryStream memoryStream);

        private void WebRequestFinished(MemoryStream memoryStream)
        {
            CreateGifAnimation(memoryStream);
        }

        private delegate void WebRequestErrorDelegate(Exception exp);

        private void WebRequestError(Exception exp)
        {
            RaiseImageFailedEvent(exp);
        }

        private void WebResponseCallback(IAsyncResult asyncResult)
        {
            WebReadState webReadState = (WebReadState)asyncResult.AsyncState;
            WebResponse webResponse;
            try
            {
                webResponse = webReadState.webRequest.EndGetResponse(asyncResult);
                webReadState.readStream = webResponse.GetResponseStream();
                webReadState.buffer = new byte[100000];
                webReadState.readStream.BeginRead(webReadState.buffer, 0, webReadState.buffer.Length, new AsyncCallback(WebReadCallback), webReadState);
            }
            catch (WebException exp)
            {
                Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestErrorDelegate(WebRequestError), exp);
            }
        }

        private void WebReadCallback(IAsyncResult asyncResult)
        {
            WebReadState webReadState = (WebReadState)asyncResult.AsyncState;
            int count = webReadState.readStream.EndRead(asyncResult);
            if (count > 0)
            {
                webReadState.memoryStream.Write(webReadState.buffer, 0, count);
                try
                {
                    webReadState.readStream.BeginRead(webReadState.buffer, 0, webReadState.buffer.Length, new AsyncCallback(WebReadCallback), webReadState);
                }
                catch (WebException exp)
                {
                    Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestErrorDelegate(WebRequestError), exp);
                }
            }
            else
            {
                Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestFinishedDelegate(WebRequestFinished), webReadState.memoryStream);
            }
        }

        private void GetGifStreamFromHttp(Uri uri)
        {
            try
            {
#pragma warning disable SYSLIB0014 // 类型或成员已过时
                WebReadState webReadState = new()
                {
                    memoryStream = new MemoryStream(),
                    webRequest = WebRequest.Create(uri)
                };
#pragma warning restore SYSLIB0014 // 类型或成员已过时
                webReadState.webRequest.Timeout = 10000;

                webReadState.webRequest.BeginGetResponse(new AsyncCallback(WebResponseCallback), webReadState);
            }
            catch (SecurityException)
            {
                CreateNonGifAnimationImage();
            }
        }

        private void ReadGifStreamSynch(Stream s)
        {
            byte[] gifData;
            MemoryStream memoryStream;
            using (s)
            {
                memoryStream = new MemoryStream((int)s.Length);
                BinaryReader br = new(s);
                gifData = br.ReadBytes((int)s.Length);
                memoryStream.Write(gifData, 0, (int)s.Length);
                memoryStream.Flush();
            }
            CreateGifAnimation(memoryStream);
        }

        private void GetGifStreamFromPack(Uri uri)
        {
            try
            {
                StreamResourceInfo streamInfo;

                if (!uri.IsAbsoluteUri)
                {
                    streamInfo = Application.GetContentStream(uri);
                    streamInfo ??= Application.GetResourceStream(uri);
                }
                else
                {
                    if (uri.GetLeftPart(UriPartial.Authority).Contains("siteoforigin"))
                    {
                        streamInfo = Application.GetRemoteStream(uri);
                    }
                    else
                    {
                        streamInfo = Application.GetContentStream(uri);
                        streamInfo ??= Application.GetResourceStream(uri);
                    }
                }
                if (streamInfo == null)
                {
                    throw new FileNotFoundException("Resource not found.", uri.ToString());
                }
                ReadGifStreamSynch(streamInfo.Stream);
            }
            catch (Exception exp)
            {
                RaiseImageFailedEvent(exp);
            }
        }
    }

    public class GifImageExceptionRoutedEventArgs : RoutedEventArgs
    {
        public Exception ErrorException;

        public GifImageExceptionRoutedEventArgs(RoutedEvent routedEvent, object obj) : base(routedEvent, obj)
        {

        }
    }

    internal class WebReadState
    {
        public WebRequest webRequest;
        public MemoryStream memoryStream;
        public Stream readStream;
        public byte[] buffer;
    }
}
