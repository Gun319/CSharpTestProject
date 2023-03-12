using System.Windows;
using System.Windows.Controls;

namespace LoadingAnimation
{
    public class ThreePoingLoading : UserControl
    {
        static ThreePoingLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThreePoingLoading), new FrameworkPropertyMetadata(typeof(ThreePoingLoading)));
        }
    }
}
