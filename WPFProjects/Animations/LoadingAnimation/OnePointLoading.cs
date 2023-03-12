using System.Windows;
using System.Windows.Controls;

namespace LoadingAnimation
{
    public class OnePointLoading : UserControl
    {

        static OnePointLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OnePointLoading), new FrameworkPropertyMetadata(typeof(OnePointLoading)));
        }
    }
}
