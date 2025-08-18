using System;

namespace CountdownProject.Models
{
    public class HistoryEntry
    {
        public DateTime When { get; set; }
        public string Letters { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string Word1 { get; set; }
        public string Word2 { get; set; }
        public int RoundScore1 { get; set; }
        public int RoundScore2 { get; set; }
        public int Total1 { get; set; }
        public int Total2 { get; set; }
    }
}
