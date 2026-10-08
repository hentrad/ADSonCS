#nullable enable
using System;
using System.Text;

class Program
{

    class Node
    {
        public int Key;
        public Node? Next;

        public Node(int key)
        {
            Key = key;
            Next = null;
        }
    }

    static Node InsertSorted(Node? head, int key)
    {
        Node newNode = new Node(key);

        if (head == null || key < head.Key)
        {
            newNode.Next = head;
            return newNode;
        }

        Node current = head;
        while (current.Next != null && current.Next.Key < key)
        {
            current = current.Next;
        }

        newNode.Next = current.Next;
        current.Next = newNode;

        return head;
    }

    static void PrintList(string title, Node? head)
    {
        Console.Write(title);

        if (head == null)
        {
            Console.WriteLine("<пусто>");
            return;
        }

        Node? current = head;
        while (current != null)
        {
            Console.Write(current.Key);
            if (current.Next != null) Console.Write(" -> ");
            current = current.Next;
        }
        Console.WriteLine();
    }
    static void SplitByKey(Node? source, int key, out Node? first, out Node? second)
    {
        first = null;
        second = null;

        if (source == null) return;

        if (source.Key == key)
        {
            first = null;
            second = source;
            return;
        }

        Node prev = source;
        Node? current = source.Next;

        while (current != null && current.Key != key)
        {
            prev = current;
            current = current.Next;
        }

        if (current == null)
        {
            first = source;
            second = null;
            return;
        }

        first = source;
        prev.Next = null;
        second = current;
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Node? head = null;

        Console.Write("количество элементов: ");

        int n;
        while (!int.TryParse(Console.ReadLine(), out n) || n < 0) {Console.Write("количество элементов: ");}

        for (int i = 0; i < n; i++)
        {
            Console.Write($"Введите элемент {i + 1}: ");
            int value;
            while (!int.TryParse(Console.ReadLine(), out value)) {Console.Write($"Введите элемент {i + 1}: ");}
            head = InsertSorted(head, value);
        }

        PrintList("Исходный упорядоченный список: ", head);

        Console.Write("Введите ключ для разделения: ");
        int key;
        while (!int.TryParse(Console.ReadLine(), out key)) { }

        SplitByKey(head, key, out Node? first, out Node? second);

        Console.WriteLine();
        PrintList("Первый список: ", first);
        PrintList("Второй список: ", second);

        if (second == null)
            Console.WriteLine($"Элемент с ключом {key} не найден. Второй список пуст.");
    }
}