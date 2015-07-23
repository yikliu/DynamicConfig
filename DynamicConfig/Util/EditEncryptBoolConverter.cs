using System;
using System.Globalization;
using System.Windows.Data;

namespace DynamicConfig.ConfigTray.Util
{
    class EditEncryptBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            bool useValue = (bool)value;
            return useValue ? "Encrypt" : "Edit";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
