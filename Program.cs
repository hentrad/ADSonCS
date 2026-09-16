using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TableSortingApp
{
    class Program
    {
        static bool valid = true;
        static void Main(string[] args)
        {
            
            List<TableRecord> records = new List<TableRecord>();

            do
            {
                Console.Clear();
                Console.WriteLine("1. Загрузить из файла data.txt");
                Console.WriteLine("2. Ввести вручную");
                Console.WriteLine("0. Выход");
                Console.Write("\nВаш выбор: ");

                string? choice = Console.ReadLine();

                if (choice == "0") return;

                if (choice == "1")
                {
                    string filePath = "data.txt";
                    if (!File.Exists(filePath))
                    {
                        filePath = Path.Combine("..", "..", "..", "data.txt");
                    }

                    if (LoadFromFile(filePath, records))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nУспешно загружено {records.Count} записей из файла.");
                        Console.ResetColor();
                        valid = false;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nНе удалось загрузить файл data.txt (не найден или пуст).");
                        Console.ResetColor();
                        Console.ReadLine();
                    }
                }
                else if (choice == "2")
                {
                    Console.Clear();
                    Console.WriteLine("Введите записи в формате: Ключ;Значение");
                    Console.WriteLine("Пример: 45;Яблоко");
                    Console.WriteLine("Для завершения введите 'end'\n");

                    InputManually(records);

                    if (records.Count > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Введено {records.Count} записей.");
                        Console.ResetColor();
                        Console.ReadLine();
                        valid = false;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nВы не ввели ни одной записи!");
                        Console.ResetColor();
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("\nНеверный ввод. Нажмите Enter...");
                    Console.ReadLine();
                }
            } while (valid);

            TableRecord[] originalTable = records.ToArray();

            valid = true;
            do
            {
                Console.Clear();
                TableSorter.PrintTable(originalTable);

                Console.WriteLine("1. Метод вставки с прямым включением");
                Console.WriteLine("2. Метод Шелла");
                Console.WriteLine("0. Выход");
                Console.Write("Ваш выбор: ");

                string? choice = Console.ReadLine();

                if (choice == "0") valid = false;

                TableRecord[] tableToSort = (TableRecord[])originalTable.Clone();

                switch (choice)
                {
                    case "1":
                        PerformSort(tableToSort, TableSorter.InsertionSort);
                        break;
                    case "2":
                        PerformSort(tableToSort, TableSorter.ShellSort);
                        break;
                    default:
                        Console.WriteLine("Неверный ввод. Нажмите Enter...");
                        Console.ReadLine();
                        break;
                }
            } while (valid);
        }

        static void PerformSort(TableRecord[] table, Action<TableRecord[]> sortMethod)
{

            sortMethod(table);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nСОРТИРОВКА ЗАВЕРШЕНА");
            Console.ResetColor();
            TableSorter.PrintTable(table, -1, -1);

            Console.ReadLine();
        }

        static bool LoadFromFile(string filename, List<TableRecord> list)
        {
            if (!File.Exists(filename)) return false;

            try
            {
                string[] lines = File.ReadAllLines(filename);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split(';');
                    if (parts.Length >= 2)
                    {
                        string keyStr = parts[0].Trim();
                        string valStr = parts[1].Trim();

                        if (int.TryParse(keyStr, out int key))
                        {
                            list.Add(new TableRecord(key, valStr));
                        }
                    }
                }
                return list.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        static void InputManually(List<TableRecord> list)
        {
            
            valid = true;
            do
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                
                if (input == null) continue;
                if (input.Trim().ToLower() == "end") valid = false;
                if (string.IsNullOrWhiteSpace(input)) continue;

                string[] parts = input.Split(';');
                if (parts.Length >= 2 && int.TryParse(parts[0].Trim(), out int key))
                {
                    list.Add(new TableRecord(key, parts[1].Trim()));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("invalid input");
                    Console.ResetColor();
                }
            } while (valid);
        }
    }
}