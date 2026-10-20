using System;

namespace HomeWorkGame.Settings
{
    /// <summary>
    /// Интерфейс настроек игры
    /// </summary>
    public interface IGameSettings
    {
        int MinNumber { get; set; }
        int MaxNumber { get; set; }
        int MaxAttempts { get; set; }
    }
}