using System.Globalization;
using Microsoft.Maui.Controls;

namespace SampleMauiAppShell;

public class IntToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue && parameter is string stringParameter)
        {
            if (int.TryParse(stringParameter, out int paramValue))
            {
                return intValue == paramValue;
            }
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && boolValue && parameter is string stringParameter)
        {
            if (int.TryParse(stringParameter, out int result))
            {
                return result;
            }
        }
        return 0;
    }
} 