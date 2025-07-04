using Microsoft.Maui.Controls;

namespace HappyHabitTracker;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
