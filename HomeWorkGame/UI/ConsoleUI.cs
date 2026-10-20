using System;
using HomeWorkGame;
using HomeWorkGame.Core;
using HomeWorkGame.Enums;
using HomeWorkGame.Settings;

namespace HomeWorkGame.UI
{
    /// <summary>
    /// Консольный интерфейс пользователя
    /// </summary>
    public class ConsoleUI : IConsoleUI
    {
        /// <summary>
        /// Отображает сообщение пользователю
        /// </summary>
        /// <param name="message">Сообщение для отображения</param>
        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Считывает предположение пользователя
        /// </summary>
        /// <returns>Число, введенное пользователем</returns>
        public int ReadGuess()
        {
            int guess;
            bool isValid = false;

            do
            {
                ShowMessage("Введите ваше предположение: ");
                string? input = Console.ReadLine();

                if (int.TryParse(input, out guess))
                {
                    isValid = true;
                }
                else
                {
                    ShowMessage("Ошибка: введите целое число");
                }
            } while (!isValid);

            return guess;
        }

        /// <summary>
        /// Отображает подсказку пользователю
        /// </summary>
        /// <param name="hint">Результат проверки попытки</param>
        public void ShowHint(GuessResult hint)
        {
            switch (hint)
            {
                case GuessResult.Correct:
                    ShowMessage("Поздравляем! Вы угадали число!");
                    break;
                case GuessResult.TooHigh:
                    ShowMessage("Загаданное число меньше вашего предположения");
                    break;
                case GuessResult.TooLow:
                    ShowMessage("Загаданное число больше вашего предположения");
                    break;
            }
        }

        /// <summary>
        /// Отображает результат игры
        /// </summary>
        /// <param name="win">True, если игрок выиграл</param>
        public void ShowGameResult(bool win)
        {
            if (win)
            {
                ShowMessage("Поздравляем! Вы выиграли!");
            }
            else
            {
                ShowMessage("Игра окончена. Попробуйте еще раз!");
            }
        }

        /// <summary>
        /// Получает настройки игры от пользователя
        /// </summary>
        /// <returns>Объект с настройками игры</returns>
        public GameSettings GetGameSettings()
        {
            var settings = new GameSettings();

            // Получаем минимальное число
            settings.MinNumber = ReadIntegerInRange("Введите нижнюю границу диапазона (minNumber): ", int.MinValue, int.MaxValue);

            // Получаем максимальное число
            settings.MaxNumber = ReadIntegerInRange("Введите верхнюю границу диапазона (maxNumber): ", settings.MinNumber + 1, int.MaxValue);

            // Получаем количество попыток
            settings.MaxAttempts = ReadIntegerInRange("Введите количество попыток (maxAttempts): ", 1, int.MaxValue);

            return settings;
        }

        /// <summary>
        /// Считывает целое число из консоли с валидацией диапазона
        /// </summary>
        /// <param name="prompt">Подсказка для пользователя</param>
        /// <param name="minValue">Минимальное допустимое значение</param>
        /// <param name="maxValue">Максимальное допустимое значение</param>
        /// <returns>Введенное пользователем целое число</returns>
        private int ReadIntegerInRange(string prompt, int minValue, int maxValue)
        {
            int value;
            bool isValid = false;

            do
            {
                ShowMessage(prompt);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out value) && value >= minValue && value <= maxValue)
                {
                    isValid = true;
                }
                else
                {
                    ShowMessage($"Ошибка: введите целое число в диапазоне от {minValue} до {maxValue}");
                }
            } while (!isValid);

            return value;
        }
    }
}