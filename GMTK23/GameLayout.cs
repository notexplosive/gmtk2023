using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Layout;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class GameLayout
{
    public RectangleF Feedback { get; private set; }

    public RectangleF Player { get; private set; }

    public RectangleF Ui { get; private set; }

    public RectangleF Game { get; private set; }

    public void Compute(Point totalSize)
    {
        var padding = 8;
        var builder = new LayoutBuilder(
            new Style(
                PaddingBetweenElements: padding,
                Orientation: Orientation.Horizontal,
                Margin: new Vector2(16, 16))
        );

        var left = builder.AddGroup(
            new Style(
                Orientation.Vertical,
                padding),
            L.FillVertical(420));
        var right = builder.AddGroup(
            new Style(
                Orientation.Vertical,
                padding),
            L.FillBoth());

        left.Add(L.FixedElement(nameof(GameLayout.Game), 420, 420));
        left.Add(L.FillBoth(nameof(GameLayout.Feedback)));

        right.Add(L.FillBoth(nameof(GameLayout.Ui)));
        right.Add(L.FillBoth(nameof(GameLayout.Player)));

        var baked = builder.Bake(totalSize);

        Game = baked.FindElement(nameof(GameLayout.Game)).Rectangle;
        Ui = baked.FindElement(nameof(GameLayout.Ui)).Rectangle;
        Player = baked.FindElement(nameof(GameLayout.Player)).Rectangle;
        Feedback = baked.FindElement(nameof(GameLayout.Feedback)).Rectangle;
    }
}
