using System.Runtime.InteropServices;
using System.Windows.Media;

namespace ColorExtractor
{
    public partial class ScreenColorPicker
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hDC, int x, int y);

        public static Color GetColorAt(int x, int y)
        {
            IntPtr deskDC = GetDC(IntPtr.Zero);
            uint colorCode = GetPixel(deskDC, x, y);
            ReleaseDC(IntPtr.Zero, deskDC);

            Color color = Color.FromArgb(255,
                (byte)(colorCode & 0x000000FF),
                (byte)((colorCode & 0x0000FF00) >> 8),
                (byte)((colorCode & 0x00FF0000) >> 16));

            return color;
        }
    }
}
