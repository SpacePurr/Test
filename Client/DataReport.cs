using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    class DataReport
    {
        public static void PrintInfo(string avg, string stdev, string mode, string median, string lp, string count)
        {
            ConsoleOutput.Print("Среднее арифметическое", avg);
            ConsoleOutput.Print("Стандартное отклонение", stdev);
            ConsoleOutput.Print("Мода", mode);
            ConsoleOutput.Print("Медиана", median);
            ConsoleOutput.Print("Количество потерянных пакетов", lp);
            ConsoleOutput.Print("Количество записей", count);
        }

        public static void PrintHelp()
        {
            ConsoleOutput.Print("--info", "Все свойства в виде таблицы");

            ConsoleOutput.Print("--count", "Количество записей");
            ConsoleOutput.Print("--avg", "Среднее арифетическое");
            ConsoleOutput.Print("--stdev", "Среднее отклонение");
            ConsoleOutput.Print("--mode", "Мода");
            ConsoleOutput.Print("--median", "Медиана");
            ConsoleOutput.Print("--lp", "Количество потерянных пакетов");
            ConsoleOutput.Print("--help", "Справка");
        }

        public static void PrintSingleValue(string header, string value) => ConsoleOutput.Print(header, value);
    }
}
