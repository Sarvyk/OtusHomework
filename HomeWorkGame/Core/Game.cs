using HomeWorkGame;
using HomeWorkGame.Enums;
using HomeWorkGame.Settings;
using HomeWorkGame.UI;
using System.IO;

namespace HomeWorkGame.Core
{
    /// <summary>
    /// Класс игры в угадайку
    /// </summary>
    public class Game : IGame
    {
        /// <summary>
        /// Целевое число, которое нужно угадать
        /// </summary>
        private int targetNumber;

        /// <summary>
        /// Оставшееся количество попыток
        /// </summary>
        private int attemptsLeft;

        /// <summary>
        /// Настройки игры
        /// </summary>
        private GameSettings settings = null!;

        /// <summary>
        /// Интерфейс пользовательского интерфейса
        /// </summary>
        private IConsoleUI consoleUI = null!;

        /// <summary>
        /// Конструктор игры с внедрением зависимостей
        /// </summary>
        /// <param name="consoleUI">Интерфейс пользовательского интерфейса</param>
        public Game(IConsoleUI consoleUI)
        {
            this.consoleUI = consoleUI;
        }

        /// <summary>
        /// Инициализация (генерация числа, сброс попыток)
        /// </summary>
        public void Start()
        {
            // Запрашиваем настройки с консоли
            settings = consoleUI.GetGameSettings();
            // Инициализация игры с настройками
            InitializeGame();
        }

        /// <summary>
        /// Инициализация игры: генерация целевого числа и сброс попыток
        /// </summary>
        private void InitializeGame()
        {
            // Генерируем случайное число в заданном диапазоне
            var random = new Random();
            targetNumber = random.Next(settings.MinNumber, settings.MaxNumber + 1);
            
            // Сбрасываем количество попыток
            attemptsLeft = settings.MaxAttempts;
        }

        /// <summary>
        /// Сравнение с загаданным числом
        /// </summary>
        /// <param name="guess">Предполагаемое число</param>
        /// <returns>Результат проверки</returns>
        public GuessResult CheckGuess(int guess)
        {
            attemptsLeft--; // Уменьшаем количество попыток
            
            if (guess == targetNumber)
            {
                return GuessResult.Correct;
            }
            else if (guess > targetNumber)
            {
                return GuessResult.TooHigh;
            }
            else
            {
                return GuessResult.TooLow;
            }
        }

        /// <summary>
        /// Проверяет, окончена ли игра
        /// </summary>
        /// <returns>True, если игра окончена</returns>
        public bool IsGameOver(GuessResult result)
        {
            // Игра окончена, если закончились попытки ИЛИ игрок угадал
            return attemptsLeft <= 0 || IsWin(result);
        }

        /// <summary>
        /// Проверяет, выиграл ли игрок
        /// </summary>
        /// <returns>True, если игрок выиграл</returns>
        public bool IsWin(GuessResult result)
        {
            return result == GuessResult.Correct;
        }
    }
}