namespace WPFUI.Converters;
public class RecordHighConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values is [double recordHigh, int recordHighYear] ? $"Record High: {recordHigh:F1} °C ({recordHighYear})" : string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
