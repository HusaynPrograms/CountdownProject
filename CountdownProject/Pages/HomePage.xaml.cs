using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace CountdownProject.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            P1Name.Text = Preferences.Get("P1Name", "Player 1");
            P2Name.Text = Preferences.Get("P2Name", "Player 2");
        }

        void SaveNames(object sender, System.EventArgs e)
        {
            Preferences.Set("P1Name", string.IsNullOrWhiteSpace(P1Name.Text) ? "Player 1" : P1Name.Text.Trim());
            Preferences.Set("P2Name", string.IsNullOrWhiteSpace(P2Name.Text) ? "Player 2" : P2Name.Text.Trim());
            SavedMsg.Text = "Saved";
        }

        async void GoGame(object sender, System.EventArgs e) => await Shell.Current.GoToAsync("//Game");
        async void GoHistory(object sender, System.EventArgs e) => await Shell.Current.GoToAsync("//History");
        async void GoSettings(object sender, System.EventArgs e) => await Shell.Current.GoToAsync("//Settings");
    }
}
