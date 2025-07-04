// Converters/DateToBoolConverter.cs
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace HappyHabitTracker.Converters
{
    public class DateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value is a DateTime and not its default/min value, return true.
            // This is a basic check to ensure we only show valid dates.
            if (value is DateTime date)
            {
                return date != DateTime.MinValue && date != DateTime.MaxValue && date.Year > 1900; // Basic validity check
            }
            return false; // Not a valid date or not a DateTime object
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}