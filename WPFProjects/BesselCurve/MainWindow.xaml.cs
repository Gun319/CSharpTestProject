using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BesselCurve
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateThumb();
        }

        private void UpdateThumb()
        {
            Point point1 = new Point((double)thumb1.GetValue(Canvas.LeftProperty) + thumb1.Width, (double)thumb1.GetValue(Canvas.TopProperty) + thumb1.Height / 2);
            Point point2 = new Point((double)thumb2.GetValue(Canvas.LeftProperty), (double)thumb2.GetValue(Canvas.TopProperty) + thumb2.Height / 2);

            path.SetValue(Canvas.LeftProperty, Math.Min(point1.X, point2.X));
            path.SetValue(Canvas.TopProperty, Math.Min(point1.Y, point2.Y));

            path.Width = Math.Abs(point1.X - point2.X);
            path.Height = Math.Abs(point1.Y - point2.Y);

            scale.ScaleX = point1.X < point2.X ? point1.Y < point2.Y ? 1 : -1 : point1.Y < point2.Y ? -1 : 1;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = (Thumb)sender;
            double nTop = Canvas.GetTop(thumb) + e.VerticalChange;
            double nLeft = Canvas.GetLeft(thumb) + e.HorizontalChange;
            Canvas.SetTop(thumb, nTop);
            Canvas.SetLeft(thumb, nLeft);
            UpdateThumb();
        }
    }
}
