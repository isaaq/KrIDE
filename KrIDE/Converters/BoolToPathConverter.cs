using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace KrIDE.Converters;

public class BoolToPathConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isDirectory)
        {
            return isDirectory ? "/Assets/folder.png" : "/Assets/file.png";
        }
        return "/Assets/file.png";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
