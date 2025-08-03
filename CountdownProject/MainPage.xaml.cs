using System;
using Microsoft.Maui.Controls;

namespace CountdownGame
{
    public partial class MainPage : ContentPage
    {
        private Random _random = new Random();

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnGenerateClicked(object sender, EventArgs e)
        {
            int target = _random.Next(100, 1000);
            TargetLabel.Text = $"Target: {target}";

            int[] numbers = GenerateNumbers();
            NumbersLabel.Text = $"Numbers: {string.Join(", ", numbers)}";
        }

        private int[] GenerateNumbers()
        {
            int[] small = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int[] large = { 25, 50, 75, 100 };

            int largeCount = _random.Next(1, 5); // 1 to 4 large numbers
            int smallCount = 6 - largeCount;

            var numbers = new int[6];
            for (int i = 0; i < largeCount; i++)
                numbers[i] = large[_random.Next(large.Length)];

            for (int i = 0; i < smallCount; i++)
                numbers[largeCount + i] = small[_random.Next(small.Length)];

            return numbers;
        }
    }
}
