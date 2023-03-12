using System.Windows;
using System.Windows.Controls;

namespace LoadingAnimation
{
    public class TwoPointLoading : UserControl
    {

        public double EllipseWidth
        {
            get { return (double)GetValue(EllipseWidthProperty); }
            set { SetValue(EllipseWidthProperty, value); }
        }
        public static readonly DependencyProperty EllipseWidthProperty =
            DependencyProperty.Register("EllipseWidth", typeof(double), typeof(HorizontalPoingLoading), new PropertyMetadata(5.0, OnEllipseDiameterChanged));

        private static void OnEllipseDiameterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is HorizontalPoingLoading loading)
            {
                loading.EllipseDiameter = (double)e.NewValue;
            }
        }

        static TwoPointLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TwoPointLoading), new FrameworkPropertyMetadata(typeof(TwoPointLoading)));
        }
    }
}
