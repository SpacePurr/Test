using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    class ConsoleOutput
    {
        public static void Print(string header, object value) => Console.WriteLine($"{header}: {value}\n");
    }
}
