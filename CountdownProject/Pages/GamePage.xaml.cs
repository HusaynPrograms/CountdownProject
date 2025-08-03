using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Maui.Controls;

namespace CountdownProject.Pages;

public partial class GamePage : ContentPage
{
    private readonly List<char> vowels = new() { 'A', 'E', 'I', 'O', 'U' };
    private readonly List<char> consonants = new()
    {
        'B','C','D','F','G','H','J','K','L','M',
        'N','P','Q','R','S','T','V','W','X','Y','Z'
    };

    private List<char> selectedLetters = new();
    private Timer countdownTimer;
    private int timeLeft = 30;

    public GamePage()
    {
        InitializeComponent();
        ResetGame();
    }

    private void ResetGame()
    {
        selectedLetters.Clear();
        LettersLabel.Text = "------";
        timeLeft = 30;
        TimerLabel.Text = timeLeft.ToString();
        VowelButton.IsEnabled = true;
        ConsonantButton.IsEnabled = true;
        StartButton.IsEnabled = false;
    }

    private void OnVowelClicked(object sender, EventArgs e)
    {
        if (selectedLetters.Count >= 9) return;

        var random = new Random();
        char vowel = vowels[random.Next(vowels.Count)];
        selectedLetters.Add(vowel);
        UpdateLettersDisplay();

        if (selectedLetters.Count >= 9)
            LockLetterButtons();
    }

    private void OnConsonantClicked(object sender, EventArgs e)
    {
        if (selectedLetters.Count >= 9) return;

        var random = new Random();
        char consonant = consonants[random.Next(consonants.Count)];
        selectedLetters.Add(consonant);
        UpdateLettersDisplay();

        if (selectedLetters.Count >= 9)
            LockLetterButtons();
    }

    private void UpdateLettersDisplay()
    {
        LettersLabel.Text = string.Join(" ", selectedLetters);
        if (selectedLetters.Count == 9)
            StartButton.IsEnabled = true;
    }

    private void LockLetterButtons()
    {
        VowelButton.IsEnabled = false;
        ConsonantButton.IsEnabled = false;
    }

    private void OnStartClicked(object sender, EventArgs e)
    {
        StartButton.IsEnabled = false;
        countdownTimer = new Timer(1000);
        countdownTimer.Elapsed += OnTimerElapsed;
        countdownTimer.Start();
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        timeLeft--;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            TimerLabel.Text = timeLeft.ToString();
            if (timeLeft <= 0)
            {
                countdownTimer.Stop();
                TimerLabel.Text = "Time’s up!";
            }
        });
    }
}
