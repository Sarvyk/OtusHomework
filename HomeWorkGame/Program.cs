using HomeWorkGame.Core;
using HomeWorkGame.Enums;
using HomeWorkGame.Settings;
using HomeWorkGame.UI;

namespace HomeWorkGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Создаем зависимости
            IConsoleUI consoleUI = new ConsoleUI();
            Game game = new Game(consoleUI);
            
            // Запускаем игру
            game.Start();
            GuessResult result = GuessResult.None;
            
            // Основной игровой цикл
            while (!game.IsGameOver(result))
            {
                // Получаем предположение от пользователя
                int guess = consoleUI.ReadGuess();
                
                // Проверяем предположение
                result = game.CheckGuess(guess);
                
                // Отображаем подсказку
                consoleUI.ShowHint(result);
            }
            
            // Отображаем результат игры
            bool win = game.IsWin(result);
            consoleUI.ShowGameResult(win);
            
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
