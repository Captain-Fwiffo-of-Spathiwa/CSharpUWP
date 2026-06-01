using System.Globalization;

namespace ProjectManagerProto.Views.Converters
{
    public class DateToTextConverter : IMultiValueConverter
    {
        /// <summary>
        /// Return a due date, no due date, or an overdue warning.
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? dueDate = values[0] as DateTime?;
            bool isComplete = values[1] as bool? ?? false;

            if (dueDate == null)
            {
                return "";// "No Due Date";
            }
            else if (dueDate.Value < DateTime.Now && !isComplete)
            {
                return $"Overdue!  →  Due: {dueDate.Value}";
            }
            else
            {
                return $"Due: {dueDate.Value}";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}