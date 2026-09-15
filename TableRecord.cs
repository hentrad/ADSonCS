using System;

namespace TableSortingApp
{
    public class TableRecord
    {
        public int Key { get; set; }
        public string Value { get; set; }

        public TableRecord(int key, string value)
        {
            Key = key;
            Value = value;
        }

        // Для удобного вывода
        public override string ToString()
        {
            return $"{Key,5} | {Value,-15}";
        }
    }
}