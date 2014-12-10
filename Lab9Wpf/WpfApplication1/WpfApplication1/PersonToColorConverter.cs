using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApplication1
{
    public class PersonToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Person p = value as Person;
            if (p == null)
            {
                return new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            if (p.Name.StartsWith("A"))
                return new SolidColorBrush(Color.FromRgb(255, 0, 0));
            return new SolidColorBrush(Color.FromRgb(0, 0, 255));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
