using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;

namespace CountdownProject.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            int secs = Preferences.Get("TimeLimit", 30);
            var idx = secs switch { 20 => 0, 30 => 1, 45 => 2, 60 => 3, _ => 1 };
            TimePicker.SelectedIndex = idx;

            var theme = Preferences.Get("Theme", "System");
            ThemePicker.SelectedIndex = theme == "Light" ? 1 : theme == "Dark" ? 2 : 0;
        }

        void Save(object sender, EventArgs e)
        {
            int secs = 30;
            if (TimePicker.SelectedIndex == 0) secs = 20;
            if (TimePicker.SelectedIndex == 1) secs = 30;
            if (TimePicker.SelectedIndex == 2) secs = 45;
            if (TimePicker.SelectedIndex == 3) secs = 60;
            Preferences.Set("TimeLimit", secs);

            string theme = ThemePicker.SelectedIndex switch
            {
                1 => "Light",
                2 => "Dark",
                _ => "System"
            };
            Preferences.Set("Theme", theme);

            Application.Current.UserAppTheme = theme switch
            {
                "Light" => AppTheme.Light,
                "Dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };

            SavedMsg.Text = "Saved";
        }
    }
}
