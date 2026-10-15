#nullable enable
using System;
using System.Text;

class Program
{
    // Узел бинарного дерева
    class Node
    {
        public int Key;
        public Node? Left;
        public Node? Right;

        public Node(int key)
        {
            Key = key;
            Left = null;
            Right = null;
        }

        public bool IsLeaf => Left == null && Right == null;
    }

    // ---------- Вставка в упорядоченное двоичное дерево ----------
    static Node Insert(Node? root, int key)
    {
        if (root == null)
            return new Node(key);

        if (key < root.Key)
            root.Left = Insert(root.Left, key);
        else if (key > root.Key)
            root.Right = Insert(root.Right, key);

        return root;
    }

    static void PrintTree(Node? root, string indent = "", bool isLeft = false)
    {
        if (root == null) return;

        // сначала правый ребёнок (он будет сверху)
        PrintTree(root.Right, indent + (isLeft ? "│   " : "    "), false);

        Console.WriteLine(indent + (isLeft ? "└── " : "┌── ") + root.Key);

        // потом левый
        PrintTree(root.Left, indent + (isLeft ? "    " : "│   "), true);
    }

    // ---------- Обходы (для контроля) ----------
    static void InOrder(Node? root)
    {
        if (root == null) return;
        InOrder(root.Left);
        Console.Write(root.Key + " ");
        InOrder(root.Right);
    }

    // ---------- Удаление ЛИСТА с заданным ключом ----------
    // Возвращает:
    //   0 — удалено успешно
    //   1 — дерево пусто
    //   2 — узел с таким ключом не найден
    //   3 — узел найден, но он НЕ лист (удаление не выполнено)
    static int DeleteLeaf(Node? root, int key, out Node? newRoot)
    {
        newRoot = root;

        if (root == null)
            return 1;

        // Спецслучай: корень — единственный узел и он лист
        if (root.Key == key && root.IsLeaf)
        {
            newRoot = null;
            return 0;
        }

        Node? parent = null;
        Node? cur = root;

        while (cur != null && cur.Key != key)
        {
            parent = cur;
            cur = key < cur.Key ? cur.Left : cur.Right;
        }

        if (cur == null)
            return 2;                 // не найден

        if (!cur.IsLeaf)
            return 3;                 // не лист — не удаляем

        // cur — лист, parent != null (корень-лист уже обработан выше)
        if (parent!.Left == cur) parent.Left = null;
        else                     parent.Right = null;

        return 0;
    }

    // ---------- Ввод дерева с клавиатуры ----------
    static Node? InputTree()
    {

        Node? root = null;
        int index = 1;

        bool valid;
        do
        {
            valid = true;

            Console.Write($"[{index}]: ");
            string? line = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
            {
                valid = false;
            }
            else if (int.TryParse(line.Trim(), out int value))
            {
                root = Insert(root, value);
                index++;
            }
        } while (valid);

        return root;
    }

    // ---------- Main ----------
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Node? root = InputTree();

        if (root == null)
        {
            Console.WriteLine("Дерево пусто.");
            return;
        }

        Console.WriteLine();
        PrintTree(root);

        Console.Write("по возрастанию: ");
        InOrder(root);
        Console.WriteLine();

        // -------- Цикл удаления --------
        bool run = true;
        do
        {
            Console.WriteLine();
            Console.Write(">>");
            string? line = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
            {
                run = false;
                continue;
            }

            if (!int.TryParse(line.Trim(), out int key))
            {
                continue;
            }

            int status = DeleteLeaf(root, key, out Node? newRoot);
            root = newRoot;

            switch (status)
            {
                case 0:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Лист {key} удалён.");
                    Console.ResetColor();
                    if (root == null) Console.WriteLine("<пусто>");
                    else PrintTree(root);

                    Console.Write("Симметричный обход: ");
                    InOrder(root);
                    Console.WriteLine();
                    break;

                case 1:
                    break;

                case 2:
                    break;

                case 3:
                    break;
            }
        } while (run);
    }
}