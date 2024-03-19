namespace WPFUI.Converters
{
    public class RecordLowConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values is [double recordLow, int recordLowYear] ? $"Record Low: {recordLow:F1} °C ({recordLowYear})" : string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
