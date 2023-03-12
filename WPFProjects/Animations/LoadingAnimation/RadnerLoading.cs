using System.Windows;
using System.Windows.Controls;

namespace LoadingAnimation
{
    public class RadnerLoading : UserControl
    {
        static RadnerLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadnerLoading), new FrameworkPropertyMetadata(typeof(RadnerLoading)));
        }
    }
}
