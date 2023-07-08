using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Layout;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class GameLayout
{
    public RectangleF Feedback { get; private set; }

    public RectangleF Player { get; private set; }

    public RectangleF Controls { get; private set; }

    public RectangleF Game { get; private set; }

    public void Compute(Point totalSize)
    {
        var globalMargin = new Vector2(8, 8);
        var padding = 4;
        var builder = new LayoutBuilder(
            new Style(
                PaddingBetweenElements: padding,
                Orientation: Orientation.Vertical,
                Margin: globalMargin)
        );

        var top = builder.AddGroup(
            new Style(
                Orientation.Horizontal,
                padding),
            L.FillHorizontal(420));


        var topLeft = top.AddGroup(
            new Style(
                Orientation.Vertical,
                padding),
            L.FillBoth());
        topLeft.Add(L.FillBoth(nameof(GameLayout.Feedback)));
        topLeft.Add(L.FillBoth(nameof(GameLayout.Player)));
        top.Add(L.FixedElement(nameof(GameLayout.Game), 420, 420));
        top.Add(L.FillBoth(nameof(GameLayout.Controls)));

        var baked = builder.Bake(totalSize);

        Game = baked.FindElement(nameof(GameLayout.Game)).Rectangle;
        Controls = baked.FindElement(nameof(GameLayout.Controls)).Rectangle;
        Player = baked.FindElement(nameof(GameLayout.Player)).Rectangle;
        Feedback = baked.FindElement(nameof(GameLayout.Feedback)).Rectangle;
    }
}
