// Converters/ProgressWidthConverter.cs
using System.Globalization;
using Microsoft.Maui.Controls;
using System; // Added for Math.Clamp

namespace HappyHabitTracker.Converters // Make sure this namespace is correct
{
    public class ProgressWidthConverter : IValueConverter, IMarkupExtension
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 'value' will be the Width of the ProgressBarTrack (a double)
            // 'parameter' will be the WeeklyProgress (a double between 0.0 and 1.0)

            if (value is double containerWidth && parameter is double progress)
            {
                // Ensure progress is clamped between 0 and 1 to prevent unexpected behavior
                progress = Math.Clamp(progress, 0.0, 1.0);

                // Calculate the fill width
                return containerWidth * progress;
            }
            return 0.0; // Return 0 if inputs are not valid
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}