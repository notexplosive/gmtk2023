using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Heatmap
{
    private readonly Dictionary<Point, HeatmapCell> _cells = new();
    private readonly float _cellSize;
    private readonly Vector2 _totalBounds;

    public Heatmap(Vector2 totalBounds, float cellSize)
    {
        _totalBounds = totalBounds;
        _cellSize = cellSize;

        var numberOfCells = _totalBounds.StraightDivide(_cellSize, _cellSize);

        for (var x = 0; x < numberOfCells.X; x++)
        {
            for (var y = 0; y < numberOfCells.Y; y++)
            {
                _cells.Add(new Point(x, y), new HeatmapCell(new RectangleF(new Vector2(x, y) * cellSize, new Vector2(cellSize))));
            }
        }
    }

    public HeatmapCell GetCellAt(Vector2 position)
    {
        if (_cells.TryGetValue(position.StraightDivide(_cellSize, _cellSize).ToPoint(), out var result))
        {
            return result;
        }

        return new HeatmapCell(RectangleF.Empty);
    }

    public void DebugDraw(Painter painter)
    {
        foreach (var cell in _cells.Values)
        {
            var color = new Color(1,1 - cell.Want / 60f,1);
            
            painter.DrawRectangle(cell.Rectangle.Inflated(-1,-1), new DrawSettings
            {
                Color = color.WithMultipliedOpacity(0.25f)
            });
        }
    }

    public IEnumerable<HeatmapCell> GetCellsWithin(RectangleF rect)
    {
        for (float x = rect.TopLeft.X; x <= rect.BottomRight.X; x += _cellSize)
        {
            for (float y = rect.TopLeft.Y; y <= rect.BottomRight.Y; y += _cellSize)
            {
                if (x > 0 && y > 0)
                {
                    yield return GetCellAt(new Vector2(x, y));
                }
            }
        }
    }

    public IEnumerable<HeatmapCell> GetCellsAlong(Vector2 start, Vector2 end)
    {
        var length = (end - start).Length();
        var numberOfSegments = length / _cellSize;
        var percentIncrement = 1f / numberOfSegments;
        HeatmapCell mostRecentCell = null;
        for (float percent = 0; percent < 1; percent += percentIncrement)
        {
            var found =  GetCellAt(Vector2Extensions.Lerp(start, end, percent));
            if (mostRecentCell != found)
            {
                yield return found;
            }

            mostRecentCell = found;
        }
    }
}
