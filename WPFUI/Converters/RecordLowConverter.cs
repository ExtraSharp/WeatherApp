namespace WPFUI.Converters
{
    public class RecordLowConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double recordLow && values[1] is int recordLowYear)
            {
                return $"Record Low: {recordLow:F1} °C ({recordLowYear})";
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
