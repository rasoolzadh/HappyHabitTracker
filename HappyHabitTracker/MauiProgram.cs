// MauiProgram.cs
using Microsoft.Extensions.Logging;
using Microcharts.Maui;
using Plugin.LocalNotification; // Add this using directive
using Plugin.LocalNotification.EventArgs; // Add this for NotificationActionEventArgs

namespace HappyHabitTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts()
                .UseLocalNotification() // Initialize the plugin

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // OPTIONAL: Add an event handler for when a notification is tapped
            // Use LocalNotificationCenter.Current
            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;


            return builder.Build();
        }

        // OPTIONAL: Event handler for notification taps
        private static void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            // This code runs when a notification is tapped by the user.
            Console.WriteLine($"Notification Tapped: Id={e.Request.NotificationId}, ActionId={e.ActionId}");

            if (Shell.Current != null)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync("//MainPage"); // Adjust route as needed
                });
            }
        }
    }
}