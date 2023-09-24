using System;
using System.Globalization;
using System.Windows.Data;

namespace ImageProcessing.Converts
{
    internal class StringSub : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            string[] valueNames = value.ToString()!.Split('\\');

            if (valueNames[^1].Length <= 10)
                return valueNames[^1];
            else
                return $"{valueNames[^1][..10]}…";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
