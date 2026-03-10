using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Helpers
{
    internal static class MarkupManager
    {
        public static ReplyKeyboardMarkup SetStandartKeyboardButtonList()
        {
            return new ReplyKeyboardMarkup(new List<KeyboardButton>()
            {
                new KeyboardButton("/addtask"),
                new KeyboardButton("/show"),
                new KeyboardButton("/report")
            });
        }
        public static ReplyKeyboardMarkup SetKeyboardCancel()
        {
            return new ReplyKeyboardMarkup(new List<KeyboardButton>
            {
                new KeyboardButton("/cancel")
            });
        }
    }
}