using System;
using System.Threading;

namespace TableSortingApp
{
    public static class TableSorter
    {
        public static void PrintTable(TableRecord[] table, int highlightIndex1 = -1, int highlightIndex2 = -1, bool animate = false)
        {
            if (animate)
            {
                Console.Clear();
            }

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

            // Задержка для анимации (1 секунда)
            if (animate)
            {
                Thread.Sleep(200); 
            }
        }

        public static void InsertionSort(TableRecord[] table)
        {
            Console.Clear();

            for (int i = 1; i < table.Length; i++)
            {
                TableRecord key = table[i];
                int j = i - 1;

                PrintTable(table, i, -1, animate: true);

                while (j >= 0 && table[j].Key > key.Key)
                {
                    table[j + 1] = table[j];
                    j--;
                    
                    PrintTable(table, j + 1, j, animate: true);
                }
                table[j + 1] = key;

                PrintTable(table, j + 1, -1, animate: true);
            }
        }

        public static void ShellSort(TableRecord[] table)
        {
            Console.Clear();

            int n = table.Length;
            
            for (int gap = n / 2; gap > 0; gap /= 2)
            {
                Console.Clear();
                Thread.Sleep(200);

                for (int i = gap; i < n; i++)
                {
                    TableRecord temp = table[i];
                    int j;
                    
                    PrintTable(table, i, i - gap, animate: true);

                    for (j = i; j >= gap && table[j - gap].Key > temp.Key; j -= gap)
                    {
                        table[j] = table[j - gap];
                        
                        PrintTable(table, j, j - gap, animate: true);
                    }
                    table[j] = temp;

                    PrintTable(table, j, -1, animate: true);
                }
            }
        }
    }
}