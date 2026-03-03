using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Helpers
{
    internal static class ReplyKeyboardManager
    {
        public static ReplyKeyboardMarkup SetReplyMarkupKeyboard(params string[] keyboardName)
        {
            List<KeyboardButton> buttons = new List<KeyboardButton>();
            foreach (string name in keyboardName)
            {
                buttons.Add(new KeyboardButton(name));
            }
            return new ReplyKeyboardMarkup(buttons);
        }
        public static ReplyKeyboardMarkup SetStandartListButton()
        {
            return new ReplyKeyboardMarkup(new List<KeyboardButton>()
            {
                new KeyboardButton("/addtask"),
                new KeyboardButton("/showalltasks"),
                new KeyboardButton("/showtasks"),
                new KeyboardButton("/report") });
        }
        public static ReplyKeyboardMarkup SetCancelButton()
        {
            return new ReplyKeyboardMarkup(new List<KeyboardButton>
            {
                new KeyboardButton("/cancel")
            });
        }
    }
}
