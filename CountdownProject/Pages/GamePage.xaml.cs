using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace CountdownProject.Pages
{
    public partial class GamePage : ContentPage
    {
        private static readonly Random Rng = new Random();
        private readonly List<char> _picked = new();
        private readonly char[] _vowelBag = CreateBag(new Dictionary<char, int> { ['A'] = 15, ['E'] = 21, ['I'] = 13, ['O'] = 13, ['U'] = 5 });
        private readonly char[] _consBag = CreateBag(new Dictionary<char, int>
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

        private System.Timers.Timer _timer;
        private int _remaining = 30;

        public GamePage()
        {
            InitializeComponent();
            ResetUi();
        }

        private static char[] CreateBag(Dictionary<char, int> weights)
        {
            var list = new List<char>();
            foreach (var kv in weights)
                for (int i = 0; i < kv.Value; i++) list.Add(kv.Key);
            return list.ToArray();
        }

        private void ResetUi()
        {
            _picked.Clear();
            LettersLabel.Text = "Pick 9 letters";
            _remaining = 30;
            TimerLabel.Text = "00:30";
            VowelButton.IsEnabled = true;
            ConsonantButton.IsEnabled = true;
            StartButton.IsEnabled = false;
            ResultLabel.Text = "";
            AnswerEntry.Text = "";
            StopTimerIfAny();
        }

        private void OnClearClicked(object sender, EventArgs e) => ResetUi();

        private void OnVowelClicked(object sender, EventArgs e)
        {
            if (_picked.Count >= 9) return;
            _picked.Add(_vowelBag[Rng.Next(_vowelBag.Length)]);
            UpdateLettersAfterPick();
        }

        private void OnConsonantClicked(object sender, EventArgs e)
        {
            if (_picked.Count >= 9) return;
            _picked.Add(_consBag[Rng.Next(_consBag.Length)]);
            UpdateLettersAfterPick();
        }

        private void UpdateLettersAfterPick()
        {
            LettersLabel.Text = string.Join(" ", _picked).ToUpperInvariant();
            if (_picked.Count == 9)
            {
                VowelButton.IsEnabled = false;
                ConsonantButton.IsEnabled = false;
                StartButton.IsEnabled = true;
            }
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
            if (_picked.Count != 9) return;
            _remaining = 30;
            TimerLabel.Text = "00:30";
            StartButton.IsEnabled = false;
            StartTimer();
        }

        private void StartTimer()
        {
            StopTimerIfAny();
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (_, __) =>
            {
                _remaining--;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TimerLabel.Text = $"00:{_remaining:00}";
                    if (_remaining <= 0)
                    {
                        StopTimerIfAny();
                        ResultLabel.Text = "Time's up.";
                        StartButton.IsEnabled = true;
                    }
                });
            };
            _timer.AutoReset = true;
            _timer.Start();
        }

        private void StopTimerIfAny()
        {
            if (_timer == null) return;
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        private void OnSubmitClicked(object sender, EventArgs e)
        {
            var word = (AnswerEntry.Text ?? "").Trim().ToUpperInvariant();
            if (word.Length == 0)
            {
                ResultLabel.Text = "Enter a word.";
                return;
            }

            if (!UsesOnlyPickedLetters(word))
            {
                ResultLabel.Text = "Uses letters not in the 9.";
                return;
            }

            ResultLabel.Text = $"OK: {word} ({word.Length})";
        }

        private bool UsesOnlyPickedLetters(string word)
        {
            var counts = new Dictionary<char, int>();
            foreach (var c in _picked)
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
