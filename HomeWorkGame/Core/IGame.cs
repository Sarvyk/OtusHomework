using HomeWorkGame;
using HomeWorkGame.Enums;

namespace HomeWorkGame.Core
{
    /// <summary>
    /// Интерфейс игры в угадайку
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Инициализация (генерация числа, сброс попыток)
        /// </summary>
        void Start();

        /// <summary>
        /// Сравнение с загаданным числом
        /// </summary>
        /// <param name="guess">Предполагаемое число</param>
        /// <returns>Результат проверки</returns>
        GuessResult CheckGuess(int guess);

        /// <summary>
        /// Проверяет, окончена ли игра
        /// </summary>
        /// <returns>True, если игра окончена</returns>
        bool IsGameOver(GuessResult result);

        /// <summary>
        /// Проверяет, выиграл ли игрок
        /// </summary>
        /// <returns>True, если игрок выиграл</returns>
        bool IsWin(GuessResult result);
    }
}