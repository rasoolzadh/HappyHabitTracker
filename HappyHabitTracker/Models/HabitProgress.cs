// Models/HabitProgress.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;

namespace HappyHabitTracker.Models
{
    public class HabitProgress : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private double _weeklyProgress;
        public double WeeklyProgress
        {
            get => _weeklyProgress;
            set => SetProperty(ref _weeklyProgress, value);
        }

        private string _weeklyPercentText = string.Empty;
        public string WeeklyPercentText
        {
            get => _weeklyPercentText;
            set => SetProperty(ref _weeklyPercentText, value);
        }

        private Color _backgroundColor = Colors.Transparent;
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (SetProperty(ref _backgroundColor, value))
                {
                    OnPropertyChanged(nameof(TextColor)); // TextColor depends on BackgroundColor
                }
            }
        }

        public Color TextColor => GetContrastColor(BackgroundColor);

        // --- NEW PROPERTIES FOR STREAK & HISTORY ---
        private int _currentStreak;
        public int CurrentStreak
        {
            get => _currentStreak;
            set => SetProperty(ref _currentStreak, value);
        }

        private int _longestStreak;
        public int LongestStreak
        {
            get => _longestStreak;
            set => SetProperty(ref _longestStreak, value);
        }

        // To store the raw dates for displaying history
        private List<DateTime> _completionDates = new List<DateTime>();
        public List<DateTime> CompletionDates
        {
            get => _completionDates;
            set => SetProperty(ref _completionDates, value);
        }
        // --- END NEW PROPERTIES ---

        private static Color GetContrastColor(Color backgroundColor)
        {
            var luminance = 0.299 * backgroundColor.Red +
                            0.587 * backgroundColor.Green +
                            0.114 * backgroundColor.Blue;
            return luminance > 0.5 ? Colors.Black : Colors.White;
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}