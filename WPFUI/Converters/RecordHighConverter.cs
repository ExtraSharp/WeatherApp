﻿namespace WPFUI.Converters;
public class RecordHighConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is double recordHigh && values[1] is int recordHighYear)
        {
            return $"Record High: {recordHigh:F1} °C ({recordHighYear})";
        }
        return string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}