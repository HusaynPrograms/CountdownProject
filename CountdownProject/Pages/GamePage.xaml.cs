using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using CountdownProject.Services;

namespace CountdownProject.Pages
{
    public partial class GamePage : ContentPage
    {
        Random rnd = new();
        List<char> letters = new();
        int timeLeft;
        bool roundOn;
        int p1Score, p2Score;

        char[] vowels = MakePool(new Dictionary<char, int>
        {
            ['A'] = 15,
            ['E'] = 21,
            ['I'] = 13,
            ['O'] = 13,
            ['U'] = 5
        });

        char[] consonants = MakePool(new Dictionary<char, int>
        {
            ['B'] = 2,
            ['C'] = 3,
            ['D'] = 6,
            ['F'] = 2,
            ['G'] = 3,
            ['H'] = 2,
            ['J'] = 1,
            ['K'] = 1,
            ['L'] = 5,
            ['M'] = 4,
            ['N'] = 8,
            ['P'] = 4,
            ['Q'] = 1,
            ['R'] = 9,
            ['S'] = 9,
            ['T'] = 9,
            ['V'] = 1,
            ['W'] = 2,
            ['X'] = 1,
            ['Y'] = 2,
            ['Z'] = 1
        });

        public GamePage()
        {
            InitializeComponent();
            NewRound();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Header.Text = $"{Preferences.Get("P1Name", "Player 1")} vs {Preferences.Get("P2Name", "Player 2")}";
            await DictionaryService.EnsureLoadedAsync();
            Player1Score.Text = $"{Preferences.Get("P1Name", "Player 1")} Score: {p1Score}";
            Player2Score.Text = $"{Preferences.Get("P2Name", "Player 2")} Score: {p2Score}";
        }

        static char[] MakePool(Dictionary<char, int> weights)
        {
            var pool = new List<char>();
            foreach (var kv in weights)
                for (int i = 0; i < kv.Value; i++) pool.Add(kv.Key);
            return pool.ToArray();
        }

        void PickVowel(object sender, EventArgs e)
        {
            if (letters.Count >= 9) return;
            letters.Add(vowels[rnd.Next(vowels.Length)]);
            ShowLetters();
        }

        void PickConsonant(object sender, EventArgs e)
        {
            if (letters.Count >= 9) return;
            letters.Add(consonants[rnd.Next(consonants.Length)]);
            ShowLetters();
        }

        void ClearLetters(object sender, EventArgs e)
        {
            letters.Clear();
            ShowLetters();
        }

        void ShowLetters()
        {
            LettersLayout.Children.Clear();
            foreach (var c in letters)
                LettersLayout.Children.Add(new Label { Text = c.ToString(), FontSize = 20, Margin = new Thickness(3, 0) });

            bool full = letters.Count == 9;
            StartButton.IsEnabled = full && !roundOn;
            VowelButton.IsEnabled = !full && !roundOn;
            ConsonantButton.IsEnabled = !full && !roundOn;
            ClearLettersButton.IsEnabled = !roundOn;
        }

        void StartRound(object sender, EventArgs e)
        {
            if (roundOn || letters.Count != 9) return;

            timeLeft = Preferences.Get("TimeLimit", 30);
            if (timeLeft < 10) timeLeft = 10;
            TimerLabel.Text = $"00:{timeLeft:00}";
            roundOn = true;

            StartButton.IsEnabled = false;
            VowelButton.IsEnabled = false;
            ConsonantButton.IsEnabled = false;
            ClearLettersButton.IsEnabled = false;

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (!roundOn) return false;
                timeLeft--;
                TimerLabel.Text = $"00:{timeLeft:00}";
                if (timeLeft <= 0)
                {
                    roundOn = false;
                    StartButton.IsEnabled = letters.Count == 9;
                    ClearLettersButton.IsEnabled = true;
                    return false;
                }
                return true;
            });
        }

        async void Submit(object sender, EventArgs e)
        {
            if (roundOn) return;

            await DictionaryService.EnsureLoadedAsync();

            var p1Name = Preferences.Get("P1Name", "Player 1");
            var p2Name = Preferences.Get("P2Name", "Player 2");

            var p1Word = Player1Input.Text?.Trim().ToUpper() ?? "";
            var p2Word = Player2Input.Text?.Trim().ToUpper() ?? "";

            bool p1Ok = CheckWord(p1Word);
            bool p2Ok = CheckWord(p2Word);

            int p1Round = p1Ok ? p1Word.Length : 0;
            int p2Round = p2Ok ? p2Word.Length : 0;

            if (p1Round > 0 && p2Round > 0 && p1Round != p2Round)
            {
                if (p1Round > p2Round) p2Round = 0;
                else p1Round = 0;
            }

            p1Score += p1Round;
            p2Score += p2Round;

            Player1Score.Text = $"{p1Name} Score: {p1Score}";
            Player2Score.Text = $"{p2Name} Score: {p2Score}";

            string p1Status = p1Word == "" ? "no word" : (p1Ok ? "valid word" : "not valid word");
            string p2Status = p2Word == "" ? "no word" : (p2Ok ? "valid word" : "not valid word");

            string winner = (p1Round == p2Round) ? "Draw" : (p1Round > p2Round ? p1Name : p2Name);

            string lettersStr = new string(letters.ToArray());

            await HistoryService.AddAsync(new Models.HistoryEntry
            {
                When = DateTime.Now,
                Letters = lettersStr,
                Player1 = p1Name,
                Player2 = p2Name,
                Word1 = p1Word,
                Word2 = p2Word,
                RoundScore1 = p1Round,
                RoundScore2 = p2Round,
                Total1 = p1Score,
                Total2 = p2Score
            });

            string msg =
                $"{p1Name}: {(p1Word == "" ? "-" : p1Word)} • {p1Status} • +{p1Round}\n" +
                $"{p2Name}: {(p2Word == "" ? "-" : p2Word)} • {p2Status} • +{p2Round}\n\n" +
                $"Winner: {winner}\n" +
                $"Totals — {p1Name}: {p1Score}  {p2Name}: {p2Score}";

            await DisplayAlert("Round Result", msg, "OK");
        }

        bool CheckWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return false;

            var available = new Dictionary<char, int>();
            foreach (var c in letters)
            {
                if (!available.ContainsKey(c)) available[c] = 0;
                available[c]++;
            }

            foreach (var ch in word)
            {
                if (!available.ContainsKey(ch) || available[ch] == 0) return false;
                available[ch]--;
            }

            return DictionaryService.IsWord(word);
        }

        void ResetRound(object sender, EventArgs e) => NewRound();

        void ResetGame(object sender, EventArgs e)
        {
            p1Score = 0;
            p2Score = 0;
            Player1Score.Text = $"{Preferences.Get("P1Name", "Player 1")} Score: 0";
            Player2Score.Text = $"{Preferences.Get("P2Name", "Player 2")} Score: 0";
            NewRound();
        }

        void NewRound()
        {
            roundOn = false;
            timeLeft = Preferences.Get("TimeLimit", 30);
            TimerLabel.Text = $"00:{timeLeft:00}";
            Player1Input.Text = "";
            Player2Input.Text = "";
            letters.Clear();
            ShowLetters();
        }
    }
}
