// AddHabitPage.xaml.cs
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Plugin.LocalNotification; // Add this using directive
using Microsoft.Maui.ApplicationModel; // Keep this for PermissionStatus, if other MAUI parts use it.

namespace HappyHabitTracker
{
    public partial class AddHabitPage : ContentPage
    {
        private Color _selectedColor = Colors.Gray;
        private const string AllHabitDataKey = "all_habit_data";
        private const string CustomHabitDataKey = "custom_habit_data";

        public AddHabitPage()
        {
            InitializeComponent();
            SelectedColorBox.Color = _selectedColor;
            ReminderTimePicker.IsVisible = ReminderSwitch.IsToggled;
        }

        private void OnColorSelected(object sender, EventArgs e)
        {
            if (sender is Button colorButton)
            {
                _selectedColor = colorButton.BackgroundColor;
                SelectedColorBox.Color = _selectedColor;
            }
        }

        private void OnReminderToggled(object sender, ToggledEventArgs e)
        {
            ReminderTimePicker.IsVisible = e.Value;
        }

        private async void OnSaveHabitClicked(object sender, EventArgs e)
        {
            string habitName = HabitNameEntry.Text?.Trim();
            string habitIcon = HabitIconEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(habitName))
            {
                await DisplayAlert("Missing Information", "Please enter a name for your habit.", "OK");
                return;
            }

            if (_selectedColor == Colors.Gray)
            {
                await DisplayAlert("Missing Information", "Please choose a color for your habit.", "OK");
                return;
            }

            string newHabitFullText = string.IsNullOrWhiteSpace(habitIcon) ? habitName : $"{habitIcon} {habitName}";

            var allHabitDataJson = Preferences.Get(AllHabitDataKey, "{}");
            var allHabitDailyCounts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(allHabitDataJson)
                                ?? new Dictionary<string, Dictionary<string, int>>();

            if (allHabitDailyCounts.Keys.Any(k => k.Equals(newHabitFullText, StringComparison.OrdinalIgnoreCase)))
            {
                await DisplayAlert("Habit Exists", $"A habit named '{newHabitFullText}' already exists. Please choose a different name or icon combination.", "OK");
                return;
            }

            bool hasReminder = ReminderSwitch.IsToggled;
            TimeSpan reminderTime = ReminderTimePicker.Time;

            int notificationId = newHabitFullText.GetHashCode();
            if (notificationId < 0) notificationId = Math.Abs(notificationId);
            if (notificationId == 0) notificationId = 1;


            var customHabitsJson = Preferences.Get(CustomHabitDataKey, "[]");
            var customHabits = JsonConvert.DeserializeObject<List<CustomHabit>>(customHabitsJson)
                               ?? new List<CustomHabit>();

            var newCustomHabit = new CustomHabit(name: habitName, icon: habitIcon, colorHex: _selectedColor.ToHex(), hasReminder: hasReminder, reminderTime: reminderTime, notificationId: notificationId);
            customHabits.Add(newCustomHabit);

            Preferences.Set(CustomHabitDataKey, JsonConvert.SerializeObject(customHabits));

            allHabitDailyCounts[newHabitFullText] = new Dictionary<string, int>();
            Preferences.Set(AllHabitDataKey, JsonConvert.SerializeObject(allHabitDailyCounts));

            if (hasReminder)
            {
                await ScheduleNotification(newCustomHabit);
            }

            MessagingCenter.Send(this, "NewHabitAdded");

            await DisplayAlert("Success!", $"Your new habit '{newHabitFullText}' has been created! 🎉", "OK");

            await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async Task ScheduleNotification(CustomHabit habit)
        {
            // Request permissions if not already granted
            // CORRECTED: RequestNotificationPermission returns bool. Check if NOT granted.
            bool isGranted = await LocalNotificationCenter.Current.RequestNotificationPermission();
            if (!isGranted) // Corrected: Directly check the boolean result
            {
                Console.WriteLine("Notification permission not granted.");
                await DisplayAlert("Permission Required", "Please enable notification permissions for Happy Habits in your device settings to receive reminders.", "OK");
                return;
            }

            var notificationRequest = new NotificationRequest
            {
                NotificationId = habit.NotificationId,
                Title = "Happy Habits Reminder!",
                Subtitle = $"Time to {habit.Name}!",
                Description = $"{habit.Icon} Don't forget your habit: {habit.Name}",
                CategoryType = NotificationCategoryType.None,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Today.Add(habit.ReminderTime),
                    RepeatType = NotificationRepeat.Daily
                }
            };

            // If the reminder time for today has already passed, schedule for tomorrow
            if (notificationRequest.Schedule.NotifyTime < DateTime.Now)
            {
                notificationRequest.Schedule.NotifyTime = notificationRequest.Schedule.NotifyTime.Value.AddDays(1);
            }

            await LocalNotificationCenter.Current.Show(notificationRequest);
            Console.WriteLine($"Scheduled notification for '{habit.Name}' at {habit.ReminderTime} with ID {habit.NotificationId}");
        }
    }
}