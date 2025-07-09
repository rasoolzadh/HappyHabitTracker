// AddHabitPage.xaml.cs
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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
        }

        private void OnColorSelected(object sender, EventArgs e)
        {
            if (sender is Button colorButton)
            {
                _selectedColor = colorButton.BackgroundColor;
                SelectedColorBox.Color = _selectedColor;
            }
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

            // --- MODIFIED: Load existing habit data with the new structure ---
            var allHabitDataJson = Preferences.Get(AllHabitDataKey, "{}");
            var allHabitDailyCounts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(allHabitDataJson)
                                ?? new Dictionary<string, Dictionary<string, int>>();

            // Check if a habit with this exact text already exists
            if (allHabitDailyCounts.Keys.Any(k => k.Equals(newHabitFullText, StringComparison.OrdinalIgnoreCase)))
            {
                await DisplayAlert("Habit Exists", $"A habit named '{newHabitFullText}' already exists. Please choose a different name or icon combination.", "OK");
                return;
            }

            // --- Save New Custom Habit Definition ---
            var customHabitsJson = Preferences.Get(CustomHabitDataKey, "[]");
            var customHabits = JsonConvert.DeserializeObject<List<CustomHabit>>(customHabitsJson)
                               ?? new List<CustomHabit>();

            var newCustomHabit = new CustomHabit(habitName, habitIcon, _selectedColor.ToHex());
            customHabits.Add(newCustomHabit);

            Preferences.Set(CustomHabitDataKey, JsonConvert.SerializeObject(customHabits));

            // --- MODIFIED: Initialize empty completion data for the new habit with the new structure ---
            // A new habit starts with an empty dictionary for its daily counts.
            allHabitDailyCounts[newHabitFullText] = new Dictionary<string, int>();
            Preferences.Set(AllHabitDataKey, JsonConvert.SerializeObject(allHabitDailyCounts));

            // Notify MainPage to reload its habit buttons
            MessagingCenter.Send(this, "NewHabitAdded");

            await DisplayAlert("Success!", $"Your new habit '{newHabitFullText}' has been created! 🎉", "OK");

            await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}