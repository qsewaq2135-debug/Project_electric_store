using EBIKES09.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace EBIKES09
{
    public class DiscountPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is Product product)
                {
                   
                    if (product.HasActivePromotion)
                    {
                        return product.DiscountedPrice.ToString("0.00₽", culture);
                    }
                    else
                    {
                        // Если акции нет, возвращаем обычную цену
                        return product.Price?.ToString("₽", culture) ?? "Цена не указана";
                    }
                }
                else
                {
                    
                    System.Diagnostics.Debug.WriteLine($"Неверный тип данных в DiscountPriceConverter: {value?.GetType().Name}");
                    return $"Неверный тип данных: {value?.GetType().Name}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в DiscountPriceConverter: {ex.Message}");
                return $"Ошибка: {ex.Message}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}