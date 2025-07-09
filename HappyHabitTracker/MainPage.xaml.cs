// MainPage.xaml.cs
using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

#if ANDROID
using Android.Media;
using AApplication = Android.App.Application;
#endif

namespace HappyHabitTracker
{
    public partial class MainPage : ContentPage
    {
        private const string LastResetKey = "last_reset_date";
        private const string AllHabitDataKey = "all_habit_data";
        private const string CustomHabitDataKey = "custom_habit_data";

#if ANDROID
        private MediaPlayer? _player;
#endif

        private static readonly List<PredefinedHabit> PredefinedHabits = new List<PredefinedHabit>
        {
            new PredefinedHabit("🧠 Mind & Learning", "📖 Read a Book", "#FFADAD"),
            new PredefinedHabit("🧠 Mind & Learning", "🧩 Solve a Puzzle", "#FFD6A5"),
            new PredefinedHabit("🧠 Mind & Learning", "🎓 Learn a Word", "#FDFFB6"),
            new PredefinedHabit("🧠 Mind & Learning", "✍️ Write Journal", "#CAFFBF"),
            new PredefinedHabit("🧠 Mind & Learning", "📅 Plan Tomorrow", "#A0E7E5"),

            new PredefinedHabit("💪 Health & Fitness", "💧 Drink Water", "#5AC8FA"),
            new PredefinedHabit("💪 Health & Fitness", "🏃 Exercise", "#55EFBD"),
            new PredefinedHabit("💪 Health & Fitness", "🍎 Eat Healthy", "#FF6B6B"),
            new PredefinedHabit("💪 Health & Fitness", "🧘 Meditate", "#A29BFE"),
            new PredefinedHabit("💪 Health & Fitness", "🛌 Sleep Early", "#FDC1C5"),
            new PredefinedHabit("💪 Health & Fitness", "📵 Limit Screen Time", "#C1C8E4"),

            new PredefinedHabit("🎨 Creativity & Fun", "🎨 Draw Something", "#FEC8D8"),
            new PredefinedHabit("🎨 Creativity & Fun", "🎶 Listen to Music", "#FDCB6E"),
            new PredefinedHabit("🎨 Creativity & Fun", "🎮 Play a Game", "#A0E7E5"),
            new PredefinedHabit("🎨 Creativity & Fun", "🎤 Sing or Dance", "#F9C6C9"),
            new PredefinedHabit("🎨 Creativity & Fun", "🎬 Watch Something Short", "#FFF5BA"),

            new PredefinedHabit("💕 Kindness & Relationships", "💬 Call a Friend", "#C1C8E4"),
            new PredefinedHabit("💕 Kindness & Relationships", "💌 Write a Thank You", "#FFB4B4"),
            new PredefinedHabit("💕 Kindness & Relationships", "🤗 Give a Hug", "#81ECEC"),
            new PredefinedHabit("💕 Kindness & Relationships", "🧃 Recycle Something", "#D6A2E8"),
            new PredefinedHabit("💕 Kindness & Relationships", "🌿 Water a Plant", "#B5EAD7"),

            new PredefinedHabit("🧹 Daily Routines", "🧹 Clean Up Room", "#FFFA65"),
            new PredefinedHabit("🧹 Daily Routines", "🛏️ Make Your Bed", "#FDC1C5"),
            new PredefinedHabit("🧹 Daily Routines", "🪥 Brush Teeth", "#9AECDB"),
            new PredefinedHabit("🧹 Daily Routines", "🧼 Wash Hands", "#D6A2E8"),
            new PredefinedHabit("🧹 Daily Routines", "🧾 Review To-Do List", "#FFD6A5"),
            new PredefinedHabit("🧹 Daily Routines", "⏰ Wake Up on Time", "#FFBDBD"),

            new PredefinedHabit("🧘 Mindfulness & Calm", "🧘 Breathe Deeply", "#A3C9F9"),
            new PredefinedHabit("🧘 Mindfulness & Calm", "🙏 Say Thank You", "#FDCB6E"),
            new PredefinedHabit("🧘 Mindfulness & Calm", "🕯️ Light a Candle", "#D3BBDD"),
            new PredefinedHabit("🧘 Mindfulness & Calm", "💤 Power Nap", "#D0F4DE"),
            new PredefinedHabit("🧘 Mindfulness & Calm", "🧠 Do Nothing (1 min)", "#FFB5E8"),

            new PredefinedHabit("🐾 Nature & Pets", "🐶 Feed the Pet", "#B0EACD"),
            new PredefinedHabit("🐾 Nature & Pets", "🐕 Walk the Dog", "#FAD0C3"),
            new PredefinedHabit("🐾 Nature & Pets", "🌤️ Watch the Sky", "#A0E7E5"),
            new PredefinedHabit("🐾 Nature & Pets", "🌳 Touch a Tree", "#CAFFBF"),
            new PredefinedHabit("🐾 Nature & Pets", "🌻 Water Flowers", "#FDFFB6"),

            new PredefinedHabit("🛠 Productivity & Growth", "✅ Finish a Task", "#FEC8D8"),
            new PredefinedHabit("🛠 Productivity & Growth", "📂 Clean a Folder", "#FFDAC1"),
            new PredefinedHabit("🛠 Productivity & Growth", "🧹 Organize Something", "#D5AAFF"),
            new PredefinedHabit("🛠 Productivity & Growth", "🧾 Check To-Do List", "#FFF5BA"),
            new PredefinedHabit("🛠 Productivity & Growth", "📋 Write a Goal", "#A3E4DB"),

            new PredefinedHabit("🧡 Self-Love & Mood", "🪞 Compliment Yourself", "#FFC3A0"),
            new PredefinedHabit("🧡 Self-Love & Mood", "💖 Smile at the Mirror", "#FFB6B9"),
            new PredefinedHabit("🧡 Self-Love & Mood", "🧼 Pamper Yourself", "#D0F4DE"),
            new PredefinedHabit("🧡 Self-Love & Mood", "📸 Take a Happy Photo", "#F9C6C9"),
            new PredefinedHabit("🧡 Self-Love & Mood", "✨ Celebrate Yourself", "#D5AAFF")
        };


        public MainPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<ProgressChartPage>(this, "ResetAllData", (sender) =>
            {
                Device.BeginInvokeOnMainThread(() => ResetHabitButtonUI());
            });
            MessagingCenter.Subscribe<AddHabitPage>(this, "NewHabitAdded", (sender) =>
            {
                Device.BeginInvokeOnMainThread(() => LoadHabitStates());
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ProgressChartPage>(this, "ResetAllData");
            MessagingCenter.Unsubscribe<AddHabitPage>(this, "NewHabitAdded");
#if ANDROID
            _player?.Release();
            _player?.Dispose();
            _player = null;
#endif
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadHabitStates();
            LoadingOverlay.IsVisible = false;
        }

        private async void OnCloseHabitPopup(object sender, EventArgs e)
        {
            await HabitPopupOverlay.FadeTo(0, 250);
            HabitPopupOverlay.IsVisible = false;
        }

        private void ResetHabitButtonUI()
        {
            Preferences.Remove(AllHabitDataKey);
            Preferences.Remove(LastResetKey);
            Preferences.Remove(CustomHabitDataKey);

            LoadHabitStates();
        }

        public void LoadHabitStates()
        {
            MainHabitLayout.Children.Clear();

            var addNewHabitButton = new Button
            {
                Text = "➕ Add New Habit",
                BackgroundColor = Color.FromHex("#80EE90"),
                TextColor = Colors.White,
                FontSize = 20,
                Padding = 15,
                Margin = new Thickness(0, 0, 0, 30),
                CornerRadius = 15,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(3, 3), Opacity = 0.3f, Radius = 5 }
            };
            addNewHabitButton.Clicked += OnAddNewHabitClicked;
            MainHabitLayout.Children.Add(addNewHabitButton);


            string today = DateTime.Today.ToString("yyyyMMdd");
            string lastReset = Preferences.Get(LastResetKey, "");

            var allHabitDataJson = Preferences.Get(AllHabitDataKey, "{}");
            var allHabitDailyCounts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(allHabitDataJson)
                                ?? new Dictionary<string, Dictionary<string, int>>();

            var customHabitsJson = Preferences.Get(CustomHabitDataKey, "[]");
            var customHabits = JsonConvert.DeserializeObject<List<CustomHabit>>(customHabitsJson)
                               ?? new List<CustomHabit>();

            if (lastReset != today)
            {
                Preferences.Set(LastResetKey, today);
            }

            var allDisplayHabits = new List<object>();
            foreach (var ph in PredefinedHabits) allDisplayHabits.Add(ph);
            foreach (var ch in customHabits) allDisplayHabits.Add(ch);


            var groupedHabits = allDisplayHabits
                .GroupBy(h => (h is PredefinedHabit ph) ? ph.Category : (h is CustomHabit ch) ? "Your Custom Habits" : "Other")
                .OrderBy(g => g.Key == "Your Custom Habits" ? 0 : 1)
                .ThenBy(g => g.Key);


            foreach (var group in groupedHabits)
            {
                MainHabitLayout.Children.Add(new Label
                {
                    Text = group.Key,
                    FontSize = 22,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#4A4A4A"),
                    Margin = new Thickness(0, 30, 0, 0)
                });

                foreach (var habitItem in group.OrderBy(h => (h is PredefinedHabit ph) ? ph.Name : (h is CustomHabit ch) ? ch.Name : ""))
                {
                    string habitName;
                    Color bgColor;
                    string fullHabitText;

                    if (habitItem is PredefinedHabit ph)
                    {
                        habitName = ph.Name;
                        bgColor = Color.FromHex(ph.ColorHex);
                        fullHabitText = ph.Name;
                    }
                    else if (habitItem is CustomHabit ch)
                    {
                        habitName = ch.Name;
                        bgColor = Color.FromHex(ch.ColorHex);
                        fullHabitText = string.IsNullOrWhiteSpace(ch.Icon) ? ch.Name : $"{ch.Icon} {ch.Name}";
                    }
                    else
                    {
                        continue;
                    }

                    var button = new Button
                    {
                        Text = fullHabitText,
                        BackgroundColor = bgColor,
                        TextColor = GetContrastColor(bgColor),
                        FontSize = 20,
                        CornerRadius = 12,
                        Padding = 15,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    button.Clicked += OnHabitClicked;

                    button.Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(3, 3), Opacity = 0.3f, Radius = 5 };

                    if (allHabitDailyCounts.TryGetValue(fullHabitText, out var dailyCountsForHabit) &&
                        dailyCountsForHabit.ContainsKey(today))
                    {
                        button.Opacity = 0.6;
                        button.BorderColor = Colors.Green;
                        button.BorderWidth = 2;
                        button.CornerRadius = 10;
                    }
                    else
                    {
                        button.Opacity = 1;
                        button.BorderWidth = 0;
                    }

                    MainHabitLayout.Children.Add(button);
                }
            }
        }

        private async void OnAddNewHabitClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AddHabitPage");
        }

        private static Color GetContrastColor(Color backgroundColor)
        {
            var luminance = 0.299 * backgroundColor.Red +
                            0.587 * backgroundColor.Green +
                            0.114 * backgroundColor.Blue;
            return luminance > 0.5 ? Colors.Black : Colors.White;
        }

        private async void OnViewProgressClicked(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;
            (sender as Button).IsEnabled = false;
            await Task.Yield();

            try
            {
                await Shell.Current.GoToAsync("ProgressChartPage");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
                (sender as Button).IsEnabled = true;
            }
        }

        private async void OnHabitClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string habitFullName = button.Text;
                string todayStr = DateTime.Today.ToString("yyyyMMdd");

                var allHabitDataJson = Preferences.Get(AllHabitDataKey, "{}");
                var allHabitDailyCounts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(allHabitDataJson)
                                ?? new Dictionary<string, Dictionary<string, int>>();

                if (!allHabitDailyCounts.TryGetValue(habitFullName, out var dailyCountsForHabit))
                {
                    dailyCountsForHabit = new Dictionary<string, int>();
                    allHabitDailyCounts[habitFullName] = dailyCountsForHabit;
                }

                dailyCountsForHabit[todayStr] = dailyCountsForHabit.GetValueOrDefault(todayStr, 0) + 1;

                Preferences.Set(AllHabitDataKey, JsonConvert.SerializeObject(allHabitDailyCounts));

                button.Opacity = 0.6;
                button.BorderColor = Colors.Green;
                button.BorderWidth = 2;
                button.CornerRadius = 10;

#if ANDROID
                PlayClickSound();
#endif
                var (totalCompleted, weekly, monthly) = GetHabitStats(allHabitDailyCounts, habitFullName);

                var habitCompletionDates = new List<DateTime>();
                if (allHabitDailyCounts.TryGetValue(habitFullName, out dailyCountsForHabit))
                {
                    foreach (var dateKey in dailyCountsForHabit.Keys)
                    {
                        if (DateTime.TryParseExact(dateKey, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dt))
                        {
                            habitCompletionDates.Add(dt);
                        }
                    }
                }

                var (currentStreak, longestStreak) = CalculateStreaks(habitCompletionDates, DateTime.Today);

                int todayTapCount = dailyCountsForHabit[todayStr];


                string message = $"{GetHabitMessage(habitFullName)}\n\n" +
                                 $"✅ Tapped today: {todayTapCount} time{(todayTapCount > 1 ? "s" : "")}\n" +
                                 $"🔥 Current Streak: {currentStreak} days\n" +
                                 $"🎯 Total: {totalCompleted} days\n" +
                                 $"📆 Last 7 days: {weekly} days\n" +
                                 $"🗓️ Last 30 days: {monthly} days";

                HabitPopupTitle.Text = "✅ Habit Complete!";
                HabitPopupMessage.Text = message;

                HabitPopupOverlay.Opacity = 0;
                HabitPopupOverlay.IsVisible = true;
                await HabitPopupOverlay.FadeTo(1, 250);
            }
        }

        private (int total, int weekly, int monthly) GetHabitStats(Dictionary<string, Dictionary<string, int>> allHabitDailyCounts, string habitFullName)
        {
            if (!allHabitDailyCounts.TryGetValue(habitFullName, out var dailyCountsForHabit))
            {
                return (0, 0, 0);
            }

            var dates = dailyCountsForHabit.Keys
                                .Select(s => DateTime.TryParseExact(s, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dt) ? dt : DateTime.MinValue)
                                .Where(d => d != DateTime.MinValue)
                                .ToList();

            DateTime sevenDaysAgo = DateTime.Today.AddDays(-6);
            DateTime thirtyDaysAgo = DateTime.Today.AddDays(-29);

            int totalCompletedDays = dates.Count;
            int last7Days = dates.Count(d => d >= sevenDaysAgo);
            int last30Days = dates.Count(d => d >= thirtyDaysAgo);

            return (totalCompletedDays, last7Days, last30Days);
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

        private string GetHabitMessage(string habit)
        {
            return habit switch
            {
                "📖 Read a Book" => "Books take you on adventures 📚✨",
                "🧩 Solve a Puzzle" => "You’re boosting your brain! 🧠",
                "🎓 Learn a Word" => "Your vocabulary just leveled up! 💬",
                "✍️ Write Journal" => "Writing clears the mind 🖋️",
                "📅 Plan Tomorrow" => "Planning brings peace!🧘",
                "💧 Drink Water" => "Hydration hero! 💦",
                "🏃 Exercise" => "You’re getting stronger! 💪",
                "🍎 Eat Healthy" => "Yum! Fueling your body 🍎",
                "🧘 Meditate" => "Inner calm achieved 🧘‍♂️",
                "🛌 Sleep Early" => "Sweet dreams! 😴",
                "📵 Limit Screen Time" => "More time for life! ⏳",
                "🎨 Draw Something" => "Unleash your creativity! 🎨",
                "🎶 Listen to Music" => "Good vibes only! 🎶",
                "🎮 Play a Game" => "Time for some fun! 🎮",
                "🎤 Sing or Dance" => "Express yourself! 🎤",
                "🎬 Watch Something Short" => "A quick break! 🎬",
                "💬 Call a Friend" => "Connections warm the heart! ❤️",
                "💌 Write a Thank You" => "Spread gratitude! 🙏",
                "🤗 Give a Hug" => "Warm hugs make the day! 🤗",
                "🧃 Recycle Something" => "Helping Mother Earth! 🌍",
                "🌿 Water a Plant" => "Nurturing life! 🌱",
                "🧹 Clean Up Room" => "Sparkling clean! ✨",
                "🛏️ Make Your Bed" => "A tidy start! 🛏️",
                "🪥 Brush Teeth" => "Fresh and clean! 😃",
                "🧼 Wash Hands" => "Hygiene hero! 🧼",
                "🧾 Review To-Do List" => "Stay organized! ✅",
                "⏰ Wake Up on Time" => "Ready for the day! ☀️",
                "🧘 Breathe Deeply" => "Finding your calm. 🌬️",
                "🙏 Say Thank You" => "Gratitude is the attitude! 🙌",
                "🕯️ Light a Candle" => "Embrace the peace. 🕯️",
                "💤 Power Nap" => "Recharge time! 🔋",
                "🧠 Do Nothing (1 min)" => "Mindful pause. 🧘‍♀️",
                "🐶 Feed the Pet" => "Happy pet, happy you! 🐾",
                "🐕 Walk the Dog" => "Best pals out and about! 🐕",
                "🌤️ Watch the Sky" => "A moment of wonder! 🌌",
                "🌳 Touch a Tree" => "Connect with nature! 🌳",
                "🌻 Water Flowers" => "Blooming beauty! 🌸",
                "✅ Finish a Task" => "Task conquered! 💪",
                "📂 Clean a Folder" => "Organized digital life! 💻",
                "🧹 Organize Something" => "Order brings peace! 🧘",
                "🧾 Check To-Do List" => "On top of things! 📋",
                "📋 Write a Goal" => "Aiming high! 🚀",
                "🪞 Compliment Yourself" => "You're amazing! ✨",
                "💖 Smile at the Mirror" => "Radiate positivity! 😊",
                "🧼 Pamper Yourself" => "Self-care is key! 🛀",
                "📸 Take a Happy Photo" => "Capture the joy! 📸",
                "✨ Celebrate Yourself" => "You deserve it! 🥳",
                _ => "Keep up the great work! 🎉"
            };
        }


#if ANDROID
        private void PlayClickSound()
        {
            try
            {
                var context = AApplication.Context;
                _player?.Release();
                _player?.Dispose();
                _player = null;

                var afd = context.Assets.OpenFd("tap.mp3");
                _player = new MediaPlayer();
                _player.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
                _player.Prepare();
                _player.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Sound Error] {ex.Message}");
            }
        }
#endif
    }

    public class PredefinedHabit
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string ColorHex { get; set; }

        public PredefinedHabit(string category, string name, string colorHex)
        {
            Category = category;
            Name = name;
            ColorHex = colorHex;
        }
    }

    public class CustomHabit
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string ColorHex { get; set; }
        // NEW PROPERTIES FOR REMINDERS (Ensured these are present)
        public bool HasReminder { get; set; } = false;
        public TimeSpan ReminderTime { get; set; } = TimeSpan.FromHours(9); // Default to 9 AM
        public int NotificationId { get; set; } // Unique ID for scheduling/cancelling notifications

        public CustomHabit(string name, string icon, string colorHex)
        {
            Name = name;
            Icon = icon;
            ColorHex = colorHex;
        }

        // Add an optional constructor for custom habits with reminders (used in AddHabitPage)
        public CustomHabit(string name, string icon, string colorHex, bool hasReminder, TimeSpan reminderTime, int notificationId)
        {
            Name = name;
            Icon = icon;
            ColorHex = colorHex;
            HasReminder = hasReminder;
            ReminderTime = reminderTime;
            NotificationId = notificationId;
        }
    }
}