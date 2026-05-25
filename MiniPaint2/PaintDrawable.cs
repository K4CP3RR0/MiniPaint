using Microsoft.Maui.Graphics;
using System.Collections.Generic;

namespace MiniPaint2;

public class PaintDrawable : IDrawable
{
    private readonly List<Stroke> _strokes = new();
    private Stroke? _currentStroke;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(dirtyRect);

        foreach (var stroke in _strokes)
            DrawStroke(canvas, stroke);

        if (_currentStroke != null)
            DrawStroke(canvas, _currentStroke);
    }

    private static void DrawStroke(ICanvas canvas, Stroke stroke)
    {
        if (stroke.Points.Count < 2) return;

        canvas.StrokeColor = stroke.Color;
        canvas.StrokeSize = stroke.Thickness;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;

        var path = new PathF();
        path.MoveTo(stroke.Points[0]);
        for (int i = 1; i < stroke.Points.Count; i++)
            path.LineTo(stroke.Points[i]);

        canvas.DrawPath(path);
    }


    public void StartStroke(PointF point, Color color, float thickness)
    {
        _currentStroke = new Stroke(color, thickness);
        _currentStroke.Points.Add(point);
    }

    public void AddPoint(PointF point)
    {
        _currentStroke?.Points.Add(point);
    }

    public void EndStroke()
    {
        if (_currentStroke != null)
        {
            _strokes.Add(_currentStroke);
            _currentStroke = null;
        }
    }

    public void Undo()
    {
        if (_strokes.Count > 0)
            _strokes.RemoveAt(_strokes.Count - 1);
    }

    public void Clear()
    {
        _strokes.Clear();
        _currentStroke = null;
    }
}

public class Stroke
{
    public Color Color { get; }
    public float Thickness { get; }
    public List<PointF> Points { get; } = new();

    public Stroke(Color color, float thickness)
    {
        Color = color;
        Thickness = thickness;
    }
}