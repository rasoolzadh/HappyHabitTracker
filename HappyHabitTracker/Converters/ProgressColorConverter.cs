// Converters/ProgressColorConverter.cs
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Diagnostics;
using System; // Required for Math.Min/Max

namespace HappyHabitTracker.Converters
{
    public class ProgressColorConverter : IValueConverter, IMarkupExtension
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Color baseColor && parameter is string type)
            {
                // Calculate the desired darker shade once.
                // This will be the color for the entire progress bar (both track and fill).
                Color darkerShade = new Color(
                    baseColor.Red * 0.7f,   // Adjust this factor (e.g., 0.5f to 0.9f) to control how dark the bar is
                    baseColor.Green * 0.7f,
                    baseColor.Blue * 0.7f);

                // Ensure it's not too dark if base color is already dark
                darkerShade = new Color(
                    Math.Max(darkerShade.Red, 0.05f), // Ensures a minimum brightness
                    Math.Max(darkerShade.Green, 0.05f),
                    Math.Max(darkerShade.Blue, 0.05f));

                Color resultColor;
                if (type == "track" || type == "fill") // Apply the same darker shade to both track and fill
                {
                    resultColor = darkerShade;
                }
                else
                {
                    // Fallback for unexpected parameters (should not happen)
                    resultColor = baseColor;
                }

                Debug.WriteLine($"Color Conversion: Base: {baseColor.ToHex()}, Type: {type}, Result: {resultColor.ToHex()}");
                return resultColor;
            }
            // Fallback for invalid input to the converter
            Debug.WriteLine($"Color Conversion: Invalid input - Value: {value}, Parameter: {parameter}");
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}