using System;

namespace ScoutAI.Entities
{
    public class Player
    {
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public int Age { get; set; }
        public decimal MarketValue { get; set; }
        public string MarketValueTrend { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public double TotalRunning { get; set; }
        public double TotalRunningKm { get; set; } // ScoutAIService uyumluluğu için eklendi
        public int PotentialScore { get; set; }
        public int InjuryRiskRate { get; set; }
        public int FatigueRate { get; set; }       // ScoutAIService uyumluluğu için eklendi
        public int EstimatedRecoveryDays { get; set; }
        public string PlayerType { get; set; }
        public bool IsCaptain { get; set; }
        public string SetPieceRole { get; set; }
        public bool IsInjured { get; set; }
    }
}