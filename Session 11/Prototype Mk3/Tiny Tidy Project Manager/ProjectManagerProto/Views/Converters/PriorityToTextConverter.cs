using ProjectManagerProto.Models;

namespace ProjectManagerProto.Views.Converters
{
    public class PriorityToTextConverter : IValueConverter
    {
        /// <summary>
        /// Set friendly Priority text based on the given value.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int priority)
            {
                if (priority == 1)  return "Top priority";      // 1 = Top
                if (priority < 4)  return "High priority";      // 2, 3 = High
                if (priority < 8)  return "Medium priority";    // 4, 5, 6, 7 = Medium
                if (priority < 10) return "Low priority";       // 8, 9 = Low
                return "";//"No priority";                      // 10 = None
            }

            return "Unknown priority";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}