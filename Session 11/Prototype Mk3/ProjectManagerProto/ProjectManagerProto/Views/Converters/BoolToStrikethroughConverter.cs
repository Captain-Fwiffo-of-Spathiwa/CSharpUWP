namespace ProjectManagerProto.Views.Converters
{
    public class BoolToStrikethroughConverter : IValueConverter
    {
        /// <summary>
        /// Set Strikethrough enabled if the given bool is true.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is bool b && b) ? TextDecorations.Strikethrough : TextDecorations.None;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}