namespace ProjectManagerProto.Views.Converters
{
    public class BoolToItalicsConverter : IValueConverter
    {
        /// <summary>
        /// Set Italics if the given bool is true.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is bool b && b) ? FontAttributes.Italic : FontAttributes.Bold;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}