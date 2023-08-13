using LibVLCSharp.Shared;

namespace MyVLCMediaPlayer.Commons
{
    internal class CommonClass
    {
        /// <summary>
        /// LibVLC
        /// </summary>
        private static LibVLC _libvlc;
        public static LibVLC VLCMedia
        {
            get
            {
                _libvlc ??= new LibVLC();
                return _libvlc;
            }
            set => _libvlc = value;
        }

        /// <summary>
        /// 网络地址
        /// </summary>
        public static string NetworkUrl { get; set; }

        /// <summary>
        /// 缓存时长
        /// </summary>
        public static int CacheTime { get; set; } = 2000;

        /// <summary>  
        /// 字符串转为UniCode码字符串 &#xea88;
        /// </summary>
        public static string StringToUnicode(string str)
        {
            // 把格式 &#xea88; 转为 \xea88            
            str = str.Replace("&#", @"0").Replace(";", "");
            return char.ConvertFromUtf32(Convert.ToInt32(str, 16));
        }
    }
}
