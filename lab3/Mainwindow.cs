#nullable enable
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace lab4;

public class MainWindow : Window
{
    private readonly BinaryTree _tree = new();

    private readonly TextBox _txtKey = new() { Width = 120, Watermark = "целое число" };
    private readonly Button _btnAdd = new() { Content = "Добавить", IsDefault = true }; // Enter = добавить
    private readonly Button _btnDelete = new() { Content = "Удалить лист" };
    private readonly Button _btnClear = new() { Content = "Очистить" };
    private readonly TreeCanvas _canvas = new();
    private readonly TextBlock _lblOrder = new() { Margin = new Thickness(8, 4) };
    private readonly TextBlock _lblStatus = new() { Margin = new Thickness(8, 4) };

    public MainWindow()
    {
        Title = "Бинарное дерево: удаление листа";
        Width = 760;
        Height = 520;
        MinWidth = 520;
        MinHeight = 360;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        BuildLayout();
        WireEvents();

        _canvas.SetTree(_tree);
        RefreshView();
        _txtKey.Focus();
    }

    private void BuildLayout()
    {
        var topPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Margin = new Thickness(8)
        };
        topPanel.Children.Add(new TextBlock { Text = "Ключ:", VerticalAlignment = VerticalAlignment.Center });
        topPanel.Children.Add(_txtKey);
        topPanel.Children.Add(_btnAdd);
        topPanel.Children.Add(_btnDelete);
        topPanel.Children.Add(_btnClear);

        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            Content = _canvas
        };

        var root = new DockPanel();
        DockPanel.SetDock(topPanel, Dock.Top);
        DockPanel.SetDock(_lblOrder, Dock.Top);
        DockPanel.SetDock(_lblStatus, Dock.Bottom);
        root.Children.Add(topPanel);
        root.Children.Add(_lblOrder);
        root.Children.Add(_lblStatus);
        root.Children.Add(scroll);

        Content = root;
    }

    private void WireEvents()
    {
        _btnAdd.Click += (_, _) => AddKey();
        _btnDelete.Click += (_, _) => DeleteKey();
        _btnClear.Click += (_, _) =>
        {
            _tree.Clear();
            SetStatus("Дерево очищено.", Brushes.Black);
            RefreshView();
        };
    }

    private bool TryReadKey(out int key)
    {
        if (int.TryParse(_txtKey.Text?.Trim(), out key))
            return true;

        SetStatus("целое число.", Brushes.Firebrick);
        _txtKey.SelectAll();
        _txtKey.Focus();
        return false;
    }

    private void AddKey()
    {
        if (!TryReadKey(out int key)) return;

        _tree.Add(key);
        SetStatus($"Ключ {key} добавлен.", Brushes.Black);
        RefreshView();

        _txtKey.Text = "";
        _txtKey.Focus();
    }

    private void DeleteKey()
    {
        if (!TryReadKey(out int key)) return;

        switch (_tree.DeleteLeaf(key))
        {
            case DeleteResult.Success:
                SetStatus($"Лист {key} удалён.", Brushes.Green);
                RefreshView();
                _txtKey.Text = "";
                break;
            case DeleteResult.TreeEmpty:
                SetStatus("Дерево пусто.", Brushes.Firebrick);
                break;
            case DeleteResult.NotFound:
                SetStatus($"Узел {key} не найден.", Brushes.Firebrick);
                break;
            case DeleteResult.NotLeaf:
                SetStatus($"Узел {key} не является листом — удаление не выполнено.", Brushes.Firebrick);
                break;
        }

        _txtKey.Focus();
        _txtKey.SelectAll();
    }

    private void RefreshView()
    {
        _canvas.Relayout();
        _lblOrder.Text = _tree.IsEmpty
            ? "Симметричный обход: <пусто>"
            : "Симметричный обход: " + string.Join(" ", _tree.InOrder());
    }

    private void SetStatus(string text, IBrush color)
    {
        _lblStatus.Foreground = color;
        _lblStatus.Text = text;
    }
}