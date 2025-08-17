using System;
using Microsoft.Maui.Controls;

namespace CountdownProject.Pages
{
    public partial class GamePage : ContentPage
    {
        private int _remainingSeconds;
        private bool _roundActive;
        private int _p1Score;
        private int _p2Score;

        public GamePage()
        {
            InitializeComponent();
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
            _remainingSeconds = 30;
            TimerLabel.Text = "00:30";
            _roundActive = true;

            Player1Input.Text = string.Empty;
            Player2Input.Text = string.Empty;

            GenerateLetters();

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (_remainingSeconds > 0 && _roundActive)
                {
                    _remainingSeconds--;
                    TimerLabel.Text = $"00:{_remainingSeconds:00}";
                    return true;
                }
                return false;
            });
        }

        private void OnSubmitClicked(object sender, EventArgs e)
        {
            if (!_roundActive)
                return;

            string p1Word = Player1Input.Text?.Trim().ToUpper() ?? "";
            string p2Word = Player2Input.Text?.Trim().ToUpper() ?? "";

            if (p1Word.Length > 0)
            {
                _p1Score += p1Word.Length;
                Player1Score.Text = $"Player 1 Score: {_p1Score}";
            }

            if (p2Word.Length > 0)
            {
                _p2Score += p2Word.Length;
                Player2Score.Text = $"Player 2 Score: {_p2Score}";
            }

            _roundActive = false;
        }

        private void GenerateLetters()
        {
            Random rand = new Random();
            LettersLayout.Children.Clear();

            for (int i = 0; i < 9; i++)
            {
                char letter = (char)('A' + rand.Next(0, 26));
                LettersLayout.Children.Add(new Label
                {
                    Text = letter.ToString(),
                    FontSize = 20,
                    Margin = new Thickness(3, 0)
                });
            }
        }
    }
}
