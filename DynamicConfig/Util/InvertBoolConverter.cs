using System;
using System.Globalization;
using System.Windows.Data;

namespace DynamicConfig.ConfigTray.Util
{
    [ValueConversion(typeof(bool), typeof(bool))]
    internal class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool original = (bool)value;
            return !original;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}