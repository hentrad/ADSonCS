#nullable enable

namespace lab4;

public class Node
{
    public int Key;
    public Node? Left;
    public Node? Right;

    public Node(int key)
    {
        Key = key;
    }

    public bool IsLeaf => Left == null && Right == null;
}

public enum DeleteResult
{
    Success,
    TreeEmpty,
    NotFound,
    NotLeaf
}

public class BinaryTree
{
    public Node? Root { get; private set; }

    public bool IsEmpty => Root == null;

    public void Clear() => Root = null;

    public void Add(int key) => Root = Insert(Root, key);

    private static Node Insert(Node? root, int key)
    {
        if (root == null)
            return new Node(key);

        if (key < root.Key)
            root.Left = Insert(root.Left, key);
        else if (key > root.Key)
            root.Right = Insert(root.Right, key);

        return root;
    }

    public List<int> InOrder()
    {
        var result = new List<int>();
        InOrder(Root, result);
        return result;
    }

    private static void InOrder(Node? node, List<int> result)
    {
        if (node == null) return;
        InOrder(node.Left, result);
        result.Add(node.Key);
        InOrder(node.Right, result);
    }

    public DeleteResult DeleteLeaf(int key)
    {
        if (Root == null)
            return DeleteResult.TreeEmpty;

        if (Root.Key == key && Root.IsLeaf)
        {
            Root = null;
            return DeleteResult.Success;
        }

        Node? parent = null;
        Node? cur = Root;

        while (cur != null && cur.Key != key)
        {
            parent = cur;
            cur = key < cur.Key ? cur.Left : cur.Right;
        }

        if (cur == null)
            return DeleteResult.NotFound;

        if (!cur.IsLeaf)
            return DeleteResult.NotLeaf;

        if (parent!.Left == cur) parent.Left = null;
        else parent.Right = null;

        return DeleteResult.Success;
    }
}