using System;
using System.Runtime.InteropServices;

namespace WindowCornerModel
{
    /// <summary>
    /// 窗口圆角呈现
    /// </summary>
    public static class WindowsCorner
    {
        private const string DWMAPI = "dwmapi.dll";
        private const string USER32 = "user32.dll";

        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_NCRENDERING_ENABLED,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_CLOAK,
            DWMWA_CLOAKED,
            DWMWA_FREEZE_REPRESENTATION,
            DWMWA_PASSIVE_UPDATE_MODE,
            DWMWA_USE_HOSTBACKDROPBRUSH,
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            /// <summary>
            /// 从 Windows 11 版本 22000 开始支持此值
            /// </summary>
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
            DWMWA_BORDER_COLOR,
            DWMWA_CAPTION_COLOR,
            DWMWA_TEXT_COLOR,
            DWMWA_VISIBLE_FRAME_BORDER_THICKNESS,
            DWMWA_SYSTEMBACKDROP_TYPE,
            DWMWA_LAST
        }

        /// <summary>
        /// 圆角模式
        /// </summary>
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            /// <summary>
            /// 让系统决定是否对窗口采用圆角设置
            /// </summary>
            DWMWCP_DEFAULT = 0,

            /// <summary>
            /// 绝不对窗口采用圆角设置
            /// </summary>
            DWMWCP_DONOTROUND = 1,

            /// <summary>
            /// 适当时采用圆角设置
            /// </summary>
            DWMWCP_ROUND = 2,

            /// <summary>
            /// 适当时可采用半径较小的圆角设置
            /// </summary>
            DWMWCP_ROUNDSMALL = 3
        }

        #region 设置窗口

        /// <summary>
        /// 设置桌面窗口管理器 (DWM) 窗口的非客户端呈现属性的值
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="dwAttribute">描述要设置的值的标志，指定为 DWMWINDOWATTRIBUTE 枚举的值</param>
        /// <param name="pvAttribute">指向包含要设置的属性值的对象的指针。 值集的类型取决于 dwAttribute 参数的值。 DWMWINDOWATTRIBUTE 枚举主题指示，在每个标志的行中，应将指针传递给 pvAttribute 参数的值类型。</param>
        /// <param name="cbAttribute">通过 pvAttribute 参数设置的属性值的大小（以字节为单位）。 值集的类型及其大小（以字节为单位）取决于 dwAttribute 参数的值。</param>
        [DllImport(DWMAPI, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

        #endregion

        #region 获取操作系统版本内部号

        /// <summary>
        /// <para>
        /// Windows 10 从内部版本 10240 开始，以内部版本 19044 结束
        /// </para>
        /// <para>
        /// Windows 11 从内部版本 22000 开始
        /// </para>
        /// </summary>
        /// <returns>true：为 windows 11，false：为 Windows 11 以前版本</returns>
        public static bool OSVersion()
        {
            OperatingSystem os = Environment.OSVersion;

            if (os.Version.Build >= 22000)
                return true;

            return false;
        }

        #endregion
    }
}
