using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace ClothingStoreApp.Converters
{
    public class QuantityPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int quantity && parameter is decimal price)
            {
                return quantity * price;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}