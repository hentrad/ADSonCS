using System;
using System.Diagnostics;
using System.Threading;

namespace TableSortingApp
{
    public static class TableSorter
    {
        private static Stopwatch _stopwatch = new Stopwatch();

        public static int IterationCount { get; private set; }
        public static int ComparisonCount { get; private set; }
        public static TimeSpan ElapsedTime => _stopwatch.Elapsed;

        public static void PrintTable(TableRecord[] table, int highlightIndex1 = -1, int highlightIndex2 = -1, bool animate = false)
        {
            if (animate)
            {
                Console.Clear();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Сравнений: {ComparisonCount} | Сдвигов: {IterationCount} | Время: {ElapsedTime.TotalSeconds:F3} сек.");
            Console.WriteLine();
            Console.ResetColor();

            for (int i = 0; i < table.Length; i++)
            {
                if (i == highlightIndex1 || i == highlightIndex2)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ResetColor();
                }

                string marker = (i == highlightIndex1) ? " <--" : "";
                Console.WriteLine($"{table[i]}{marker}");
            }
            Console.ResetColor();

            if (animate)
            {
                Thread.Sleep(200); 
            }
        }

        public static void InsertionSort(TableRecord[] table)
        {
            Console.Clear();
            IterationCount = 0;
            ComparisonCount = 0;
            _stopwatch.Restart();

            PrintTable(table, -1, -1, animate: true);

            for (int i = 1; i < table.Length; i++)
            {
                TableRecord key = table[i];
                int j = i - 1;

                while (j >= 0)
                {
                    ComparisonCount++;
                    PrintTable(table, j, i, animate: true);

                    if (table[j].Key > key.Key)
                    {
                        IterationCount++;
                        table[j + 1] = table[j];
                        j--;
                        PrintTable(table, j + 1, j, animate: true);
                    }
                    else
                    {
                        break;
                    }
                }
                table[j + 1] = key;
            }

            _stopwatch.Stop();
        }

        public static void ShellSort(TableRecord[] table)
        {
            Console.Clear();
            IterationCount = 0;
            ComparisonCount = 0;
            _stopwatch.Restart();

            PrintTable(table, -1, -1, animate: true);

            int n = table.Length;

            for (int gap = n / 2; gap > 0; gap /= 2)
            {
                for (int i = gap; i < n; i++)
                {
                    TableRecord temp = table[i];
                    int j = i;

                    while (j >= gap)
                    {
                        ComparisonCount++;
                        PrintTable(table, j, j - gap, animate: true);

                        if (table[j - gap].Key > temp.Key)
                        {
                            IterationCount++;
                            int prevJ = j;
                            table[j] = table[j - gap];
                            j -= gap;
                            PrintTable(table, prevJ, j, animate: true);
                        }
                        else
                        {
                            break;
                        }
                    }
                    table[j] = temp;
                }
            }

            _stopwatch.Stop();
        }
    }
}