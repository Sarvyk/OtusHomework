using System;

namespace HomeWorkGame.Settings
{
    public class GameSettings : IGameSettings
    {
        public int MinNumber { get; set; }
        public int MaxNumber { get; set; }
        public int MaxAttempts { get; set; }
    }
}