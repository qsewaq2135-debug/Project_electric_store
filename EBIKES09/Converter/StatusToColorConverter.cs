using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace EBIKES09
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.Gray;

            var status = value.ToString().ToLower();

            switch (status)
            {
                case "новый":
                    return new SolidColorBrush(Color.FromRgb(33, 150, 243)); 
                case "в обработке":
                    return new SolidColorBrush(Color.FromRgb(255, 152, 0)); 
                case "готов":
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80)); 
                case "завершён":
                    return new SolidColorBrush(Color.FromRgb(106, 27, 154));
                case "отменён":
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54)); 
                default:
                    return new SolidColorBrush(Color.FromRgb(158, 158, 158)); 
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
