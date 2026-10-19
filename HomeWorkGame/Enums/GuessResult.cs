using System;

namespace HomeWorkGame.Enums
{
    /// <summary>
    /// Результат проверки попытки угадать число
    /// </summary>
    public enum GuessResult
    {
        None,
        /// <summary>
        /// Угадал
        /// </summary>
        Correct,
        /// <summary>
        /// Слишком много
        /// </summary>
        TooHigh,
        /// <summary>
        /// Слишком мало
        /// </summary>
        TooLow
    }
}