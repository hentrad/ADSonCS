#nullable enable
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace lab4;

public class TreeCanvas : Control
{
    private const double Radius = 18;
    private const double StepX = 44;
    private const double StepY = 64;
    private const double Pad = 30;

    private BinaryTree? _tree;
    private readonly Dictionary<Node, Point> _positions = new();
    private Size _size = new(200, 100);

    public void SetTree(BinaryTree tree)
    {
        _tree = tree;
        Relayout();
    }

    public void Relayout()
    {
        _positions.Clear();

        int col = 0, maxDepth = 0;
        Place(_tree?.Root, 0, ref col, ref maxDepth);

        _size = col == 0
            ? new Size(200, 100)
            : new Size(col * StepX + Pad * 2, (maxDepth + 1) * StepY + Pad);

        InvalidateMeasure();
        InvalidateVisual();
    }

    private void Place(Node? node, int depth, ref int col, ref int maxDepth)
    {
        if (node == null) return;

        Place(node.Left, depth + 1, ref col, ref maxDepth);

        _positions[node] = new Point(Pad + col * StepX + StepX / 2, Pad + depth * StepY);
        col++;
        if (depth > maxDepth) maxDepth = depth;

        Place(node.Right, depth + 1, ref col, ref maxDepth);
    }

    protected override Size MeasureOverride(Size availableSize) => _size;

    public override void Render(DrawingContext ctx)
    {
        ctx.DrawRectangle(Brushes.White, null, new Rect(Bounds.Size));

        if (_tree?.Root == null)
        {
            DrawText(ctx, "Дерево пусто", new Point(Pad, Pad), Brushes.Gray, false);
            return;
        }

        var linePen = new Pen(Brushes.DimGray, 2);
        DrawEdges(ctx, _tree.Root, linePen);
        DrawNodes(ctx, _tree.Root);
    }

    private void DrawEdges(DrawingContext ctx, Node? node, Pen pen)
    {
        if (node == null) return;

        foreach (var child in new[] { node.Left, node.Right })
        {
            if (child == null) continue;
            ctx.DrawLine(pen, _positions[node], _positions[child]);
            DrawEdges(ctx, child, pen);
        }
    }

    private void DrawNodes(DrawingContext ctx, Node? node)
    {
        if (node == null) return;

        var p = _positions[node];

        //  подсвечены 
        var fill = node.IsLeaf ? Brushes.PaleGreen : Brushes.LightSkyBlue;
        ctx.DrawEllipse(fill, new Pen(Brushes.DimGray, 2), p, Radius, Radius);
        DrawText(ctx, node.Key.ToString(), p, Brushes.Black, true);

        DrawNodes(ctx, node.Left);
        DrawNodes(ctx, node.Right);
    }

    private static void DrawText(DrawingContext ctx, string text, Point at, IBrush brush, bool centered)
    {
        var ft = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                                   Typeface.Default, 14, brush);
        var origin = centered ? new Point(at.X - ft.Width / 2, at.Y - ft.Height / 2) : at;
        ctx.DrawText(ft, origin);
    }
}