// ProgressChartPage.xaml.cs
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using HappyHabitTracker.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace HappyHabitTracker
{
    [QueryProperty(nameof(HabitName), "habitName")]
    public partial class ProgressChartPage : ContentPage
    {
        public ObservableCollection<HabitProgress> HabitsProgress { get; } = new();

        private const string AllHabitDataKey = "all_habit_data";
        private const string CustomHabitDataKey = "custom_habit_data";

        private static readonly List<Tuple<string, string>> PredefinedHabitsColorMap = new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("📖 Read a Book", "#FFADAD"),
            new Tuple<string, string>("🧩 Solve a Puzzle", "#FFD6A5"),
            new Tuple<string, string>("🎓 Learn a Word", "#FDFFB6"),
            new Tuple<string, string>("✍️ Write Journal", "#CAFFBF"),
            new Tuple<string, string>("📅 Plan Tomorrow", "#A0E7E5"),

            new Tuple<string, string>("💧 Drink Water", "#5AC8FA"),
            new Tuple<string, string>("🏃 Exercise", "#55EFBD"),
            new Tuple<string, string>("🍎 Eat Healthy", "#FF6B6B"),
            new Tuple<string, string>("🧘 Meditate", "#A29BFE"),
            new Tuple<string, string>("🛌 Sleep Early", "#FDC1C5"),
            new Tuple<string, string>("📵 Limit Screen Time", "#C1C8E4"),

            new Tuple<string, string>("🎨 Draw Something", "#FEC8D8"),
            new Tuple<string, string>("🎶 Listen to Music", "#FDCB6E"),
            new Tuple<string, string>("🎮 Play a Game", "#A0E7E5"),
            new Tuple<string, string>("🎤 Sing or Dance", "#F9C6C9"),
            new Tuple<string, string>("🎬 Watch Something Short", "#FFF5BA"),

            new Tuple<string, string>("💬 Call a Friend", "#C1C8E4"),
            new Tuple<string, string>("💌 Write a Thank You", "#FFB4B4"),
            new Tuple<string, string>("🤗 Give a Hug", "#81ECEC"),
            new Tuple<string, string>("🧃 Recycle Something", "#D6A2E8"),
            new Tuple<string, string>("🌿 Water a Plant", "#B5EAD7"),

            new Tuple<string, string>("🧹 Clean Up Room", "#FFFA65"),
            new Tuple<string, string>("🛏️ Make Your Bed", "#FDC1C5"),
            new Tuple<string, string>("🪥 Brush Teeth", "#9AECDB"),
            new Tuple<string, string>("🧼 Wash Hands", "#D6A2E8"),
            new Tuple<string, string>("🧾 Review To-Do List", "#FFD6A5"),
            new Tuple<string, string>("⏰ Wake Up on Time", "#FFBDBD"),

            new Tuple<string, string>("🧘 Breathe Deeply", "#A3C9F9"),
            new Tuple<string, string>("🙏 Say Thank You", "#FDCB6E"),
            new Tuple<string, string>("🕯️ Light a Candle", "#D3BBDD"),
            new Tuple<string, string>("💤 Power Nap", "#D0F4DE"),
            new Tuple<string, string>("🧠 Do Nothing (1 min)", "#FFB5E8"),

            new Tuple<string, string>("🐶 Feed the Pet", "#B0EACD"),
            new Tuple<string, string>("🐕 Walk the Dog", "#FAD0C3"),
            new Tuple<string, string>("🌤️ Watch the Sky", "#A0E7E5"),
            new Tuple<string, string>("🌳 Touch a Tree", "#CAFFBF"),
            new Tuple<string, string>("🌻 Water Flowers", "#FDFFB6"),

            new Tuple<string, string>("✅ Finish a Task", "#FEC8D8"),
            new Tuple<string, string>("📂 Clean a Folder", "#FFDAC1"),
            new Tuple<string, string>("🧹 Organize Something", "#D5AAFF"),
            new Tuple<string, string>("🧾 Check To-Do List", "#FFF5BA"),
            new Tuple<string, string>("📋 Write a Goal", "#A3E4DB"),

            new Tuple<string, string>("🪞 Compliment Yourself", "#FFC3A0"),
            new Tuple<string, string>("💖 Smile at the Mirror", "#FFB6B9"),
            new Tuple<string, string>("🧼 Pamper Yourself", "#D0F4DE"),
            new Tuple<string, string>("📸 Take a Happy Photo", "#F9C6C9"),
            new Tuple<string, string>("✨ Celebrate Yourself", "#D5AAFF")
        };


        private string _habitName;
        public string HabitName
        {
            get => _habitName;
            set
            {
                if (_habitName != value)
                {
                    _habitName = value;
                }
            }
        }

        public ProgressChartPage()
        {
            InitializeComponent();
            BindingContext = this;
            HabitsProgressCollection.ItemsSource = HabitsProgress;
            Title = "Habit Progress";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var combinedHabitColorMap = GetCombinedHabitColorMap();
            var allHabitDailyCounts = GetAllHabitCompletionData();

            List<string> habitsToDisplay;

            if (!string.IsNullOrEmpty(HabitName))
            {
                habitsToDisplay = new List<string> { HabitName };
            }
            else
            {
                habitsToDisplay = allHabitDailyCounts.Keys.ToList();
            }

            await LoadHabitProgressForDisplay(habitsToDisplay, combinedHabitColorMap, allHabitDailyCounts);
        }

        private async Task LoadHabitProgressForDisplay(List<string> habitsToDisplay, Dictionary<string, Color> habitColorMap, Dictionary<string, Dictionary<string, int>> allHabitDailyCounts)
        {
            HabitsProgress.Clear();

            await Task.Run(() =>
            {
                DateTime today = DateTime.Today;
                DateTime sevenDaysAgo = today.AddDays(-6);

                var newHabitProgressList = new List<HabitProgress>();

                foreach (var habitName in habitsToDisplay)
                {
                    if (habitColorMap.TryGetValue(habitName, out Color bgColor))
                    {
                        allHabitDailyCounts.TryGetValue(habitName, out var dailyCountsForHabit);
                        dailyCountsForHabit ??= new Dictionary<string, int>();

                        var completionDates = new List<DateTime>();
                        foreach (var dateKey in dailyCountsForHabit.Keys)
                        {
                            if (DateTime.TryParseExact(dateKey, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dt))
                            {
                                completionDates.Add(dt);
                            }
                        }

                        var (currentStreak, longestStreak) = CalculateStreaks(completionDates, today);

                        int completedDaysLast7 = completionDates.Count(d => d >= sevenDaysAgo);
                        double progress = completedDaysLast7 / 7.0;

                        newHabitProgressList.Add(new HabitProgress
                        {
                            Name = habitName,
                            WeeklyProgress = progress,
                            WeeklyPercentText = $"{completedDaysLast7}/7 days",
                            BackgroundColor = bgColor,
                            CurrentStreak = currentStreak,
                            LongestStreak = longestStreak,
                            CompletionDates = completionDates.OrderByDescending(d => d).Take(7).ToList()
                        });
                    }
                    else
                    {
                        Debug.WriteLine($"Error: Color not found for habit '{habitName}'. Displaying with default gray.");
                        newHabitProgressList.Add(new HabitProgress
                        {
                            Name = habitName,
                            WeeklyProgress = 0,
                            WeeklyPercentText = "0/7 days",
                            BackgroundColor = Colors.Gray,
                            CurrentStreak = 0,
                            LongestStreak = 0,
                            CompletionDates = new List<DateTime>()
                        });
                    }
                }

                newHabitProgressList = newHabitProgressList.OrderBy(hp => hp.Name).ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var hp in newHabitProgressList)
                    {
                        HabitsProgress.Add(hp);
                    }

                    // --- MODIFIED LOGIC FOR SETTING TITLE AND HABIT COUNT LABEL ---
                    if (habitsToDisplay.Count == 1)
                    {
                        Title = $"{habitsToDisplay[0]} Progress";
                        HabitCountLabel.Text = ""; // Clear for single habit view
                    }
                    else // This is the "My Tracked Habits 📈" view
                    {
                        Title = "My Tracked Habits 📈";
                        HabitCountLabel.Text = $"({newHabitProgressList.Count} habits total)"; // Display actual count of loaded habits
                    }
                    // --- END MODIFIED LOGIC ---

                    Debug.WriteLine($"Loaded {HabitsProgress.Count} habits into UI.");
                });
            });
        }

        private async void OnResetProgressClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Reset Progress",
                "Are you sure you want to clear ALL habit data? This will affect all habits and cannot be undone.", "Yes", "Cancel");
            if (!confirm) return;

            Preferences.Remove(AllHabitDataKey);
            Preferences.Remove("last_reset_date");
            Preferences.Remove(CustomHabitDataKey);

            MessagingCenter.Send(this, "ResetAllData");

            var combinedHabitColorMap = GetCombinedHabitColorMap();
            var allHabitDailyCounts = GetAllHabitCompletionData();

            if (!string.IsNullOrEmpty(HabitName))
            {
                await LoadHabitProgressForDisplay(new List<string> { HabitName }, combinedHabitColorMap, allHabitDailyCounts);
            }
            else
            {
                await LoadHabitProgressForDisplay(allHabitDailyCounts.Keys.ToList(), combinedHabitColorMap, allHabitDailyCounts);
            }
            await DisplayAlert("Progress Reset", "All data has been cleared successfully! 🧹", "OK");
        }


        private Dictionary<string, Color> GetCombinedHabitColorMap()
        {
            var allHabitColorMap = new Dictionary<string, Color>();

            foreach (var phTuple in PredefinedHabitsColorMap)
            {
                allHabitColorMap[phTuple.Item1] = Color.FromHex(phTuple.Item2);
            }

            var customHabitsJson = Preferences.Get(CustomHabitDataKey, "[]");
            var customHabits = JsonConvert.DeserializeObject<List<CustomHabit>>(customHabitsJson)
                               ?? new List<CustomHabit>();
            foreach (var ch in customHabits)
            {
                string fullText = string.IsNullOrWhiteSpace(ch.Icon) ? ch.Name : $"{ch.Icon} {ch.Name}";
                allHabitColorMap[fullText] = Color.FromHex(ch.ColorHex);
            }

            return allHabitColorMap;
        }

        private Dictionary<string, Dictionary<string, int>> GetAllHabitCompletionData()
        {
            var allHabitDataJson = Preferences.Get(AllHabitDataKey, "{}");
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(allHabitDataJson)
                   ?? new Dictionary<string, Dictionary<string, int>>();
        }

        private (int currentStreak, int longestStreak) CalculateStreaks(List<DateTime> completionDates, DateTime today)
        {
            if (!completionDates.Any())
            {
                return (0, 0);
            }

            var distinctSortedDates = completionDates.Select(d => d.Date).Distinct().OrderBy(d => d).ToList();

            int currentStreak = 0;
            if (distinctSortedDates.Any(d => d == today.Date))
            {
                currentStreak = 1;
                DateTime checkDate = today.AddDays(-1);
                while (distinctSortedDates.Any(d => d == checkDate.Date))
                {
                    currentStreak++;
                    checkDate = checkDate.AddDays(-1);
                }
            }
            else if (distinctSortedDates.Any(d => d == today.AddDays(-1).Date))
            {
                currentStreak = 0;
            }

            int longestStreak = 0;
            int tempStreak = 0;
            for (int i = 0; i < distinctSortedDates.Count; i++)
            {
                if (i == 0 || distinctSortedDates[i] == distinctSortedDates[i - 1].AddDays(1))
                {
                    tempStreak++;
                }
                else
                {
                    tempStreak = 1;
                }
                longestStreak = Math.Max(longestStreak, tempStreak);
            }

            longestStreak = Math.Max(longestStreak, currentStreak);

            return (currentStreak, longestStreak);
        }
    }
}