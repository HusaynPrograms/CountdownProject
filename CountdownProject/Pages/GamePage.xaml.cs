using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace CountdownProject.Pages
{
    public partial class GamePage : ContentPage
    {
        private static readonly Random rng = new();
        private readonly List<char> pickedLetters = new();
        private readonly char[] vowelsPool = BuildPool(new Dictionary<char, int> { ['A'] = 15, ['E'] = 21, ['I'] = 13, ['O'] = 13, ['U'] = 5 });
        private readonly char[] consonantsPool = BuildPool(new Dictionary<char, int>
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

        private DispatcherTimer roundTimer;
        private int secondsLeft = 30;

        public GamePage()
        {
            InitializeComponent();
            ResetRound();
        }

        private static char[] BuildPool(Dictionary<char, int> weights)
        {
            var list = new List<char>();
            foreach (var kv in weights)
            {
                for (int i = 0; i < kv.Value; i++) list.Add(kv.Key);
            }
            return list.ToArray();
        }

        private void ResetRound()
        {
            pickedLetters.Clear();
            LettersLabel.Text = "Pick 9 letters";
            secondsLeft = 30;
            TimerLabel.Text = "00:30";
            VowelButton.IsEnabled = true;
            ConsonantButton.IsEnabled = true;
            StartButton.IsEnabled = false;
            AnswerEntry.IsEnabled = false;
            SubmitButton.IsEnabled = false;
            ResultLabel.Text = "";
            AnswerEntry.Text = "";
            StopTimer();
        }

        private void OnClearClicked(object sender, EventArgs e) => ResetRound();

        private void OnVowelClicked(object sender, EventArgs e)
        {
            if (pickedLetters.Count >= 9) return;
            pickedLetters.Add(vowelsPool[rng.Next(vowelsPool.Length)]);
            AfterPick();
        }

        private void OnConsonantClicked(object sender, EventArgs e)
        {
            if (pickedLetters.Count >= 9) return;
            pickedLetters.Add(consonantsPool[rng.Next(consonantsPool.Length)]);
            AfterPick();
        }

        private void AfterPick()
        {
            LettersLabel.Text = string.Join(" ", pickedLetters).ToUpperInvariant();
            if (pickedLetters.Count == 9)
            {
                VowelButton.IsEnabled = false;
                ConsonantButton.IsEnabled = false;
                StartButton.IsEnabled = true;
            }
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
            if (pickedLetters.Count != 9) return;
            secondsLeft = 30;
            TimerLabel.Text = "00:30";
            StartButton.IsEnabled = false;
            AnswerEntry.IsEnabled = true;
            SubmitButton.IsEnabled = true;
            StartTimer();
        }

        private void StartTimer()
        {
            StopTimer();
            roundTimer = Dispatcher.CreateTimer();
            roundTimer.Interval = TimeSpan.FromSeconds(1);
            roundTimer.Tick += OnTick;
            roundTimer.Start();
        }

        private void StopTimer()
        {
            if (roundTimer == null) return;
            roundTimer.Stop();
            roundTimer.Tick -= OnTick;
            roundTimer = null;
        }

        private void OnTick(object sender, EventArgs e)
        {
            secondsLeft--;
            TimerLabel.Text = $"00:{secondsLeft:00}";
            if (secondsLeft <= 0)
            {
                StopTimer();
                ResultLabel.Text = "Time's up.";
                StartButton.IsEnabled = true;
                AnswerEntry.IsEnabled = false;
                SubmitButton.IsEnabled = false;
            }
        }

        private void OnSubmitClicked(object sender, EventArgs e)
        {
            var word = (AnswerEntry.Text ?? "").Trim().ToUpperInvariant();
            if (word.Length == 0)
            {
                ResultLabel.Text = "Enter a word.";
                return;
            }

            if (!FitsPickedLetters(word))
            {
                ResultLabel.Text = "Uses letters not in the 9.";
                return;
            }

            ResultLabel.Text = $"OK: {word} ({word.Length})";
        }

        private bool FitsPickedLetters(string word)
        {
            var counts = new Dictionary<char, int>();
            foreach (var c in pickedLetters)
            {
                if (!counts.ContainsKey(c)) counts[c] = 0;
                counts[c]++;
            }
            foreach (var ch in word)
            {
                if (!counts.ContainsKey(ch) || counts[ch] == 0) return false;
                counts[ch]--;
            }
            return true;
        }
    }
}
