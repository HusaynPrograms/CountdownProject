using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using CountdownProject.Models;
using CountdownProject.Services;

namespace CountdownProject.Pages
{
    public partial class HistoryPage : ContentPage
    {
        public HistoryPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAsync();
        }

        async Task LoadAsync()
        {
            List.ItemsSource = await HistoryService.GetAllAsync();
        }

        async void RefreshList(object sender, System.EventArgs e) => await LoadAsync();

        async void ClearAll(object sender, System.EventArgs e)
        {
            bool ok = await DisplayAlert("Clear history", "Delete all history entries?", "Yes", "No");
            if (!ok) return;
            await HistoryService.ClearAsync();
            await LoadAsync();
        }
    }
}
