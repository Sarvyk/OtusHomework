using System;
using HomeWorkGame.Enums;
using HomeWorkGame.Settings;

namespace HomeWorkGame.UI
{
    /// <summary>
    /// Интерфейс консольного интерфейса пользователя
    /// </summary>
    public interface IConsoleUI
    {
        /// <summary>
        /// Отображает сообщение пользователю
        /// </summary>
        /// <param name="message">Сообщение для отображения</param>
        void ShowMessage(string message);

        /// <summary>
        /// Считывает предположение пользователя
        /// </summary>
        /// <returns>Число, введенное пользователем</returns>
        int ReadGuess();

        /// <summary>
        /// Отображает подсказку пользователю
        /// </summary>
        /// <param name="hint">Результат проверки попытки</param>
        void ShowHint(GuessResult hint);

        /// <summary>
        /// Отображает результат игры
        /// </summary>
        /// <param name="win">True, если игрок выиграл</param>
        void ShowGameResult(bool win);

        /// <summary>
        /// Получает настройки игры от пользователя
        /// </summary>
        /// <returns>Объект с настройками игры</returns>
        GameSettings GetGameSettings();
    }
}