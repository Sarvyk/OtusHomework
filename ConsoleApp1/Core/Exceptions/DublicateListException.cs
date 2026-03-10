using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Core.Exceptions
{
    internal class DublicateListException : Exception
    {
        public DublicateListException(string list) : base($"Лист '{list}' уже существует!")
        { }
    }
}
