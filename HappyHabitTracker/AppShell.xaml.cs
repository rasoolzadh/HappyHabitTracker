// AppShell.xaml.cs
using HappyHabitTracker; // Ensure this is present
using Microsoft.Maui.Controls;

namespace HappyHabitTracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register your pages for navigation
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(ProgressChartPage), typeof(ProgressChartPage));
            Routing.RegisterRoute(nameof(AddHabitPage), typeof(AddHabitPage)); // Corrected syntax here
        }
    }
}