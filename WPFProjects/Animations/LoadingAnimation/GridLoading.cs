using System.Windows;
using System.Windows.Controls;

namespace LoadingAnimation
{
    public class GridLoading : UserControl
    {
        static GridLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridLoading), new FrameworkPropertyMetadata(typeof(GridLoading)));
        }
    }
}
