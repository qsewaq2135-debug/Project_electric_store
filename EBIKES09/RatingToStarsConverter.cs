using System;
using System.Globalization;
using System.Windows.Data;

namespace EBIKES09
{
    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal rating)
            {
                
                int fullStars = (int)Math.Round(rating, MidpointRounding.AwayFromZero);

                
                fullStars = Math.Min(fullStars, 5);

               
                return new string('★', fullStars) + new string('☆', 5 - fullStars);
            }
            return "Нет оценок";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}