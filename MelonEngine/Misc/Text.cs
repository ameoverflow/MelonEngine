using System.Drawing;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering;
using Color = Chroma.Graphics.Color;

namespace MelonEngine;

public static class Text
{
    public static void RenderTextWithShadow(RenderContext ctx, IFontProvider font, string text, float x, float y, Color color)
    {
        ctx.DrawString(font, text, x + 1, y + 1, Color.Black);
        ctx.DrawString(font, text, x, y, color);
    }
}