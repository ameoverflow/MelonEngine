using Chroma.Graphics;

namespace MelonEngine;

public static class DebugMenu
{
    public static bool Enabled { get; set; }
    public static List<int> FPSValues = Enumerable.Repeat(0, 200).ToList();
    private static Random rnd = new Random();
    public static void Draw(RenderContext context)
    {
        context.Rectangle(ShapeMode.Fill, 1, 1, 320, 358, Color.DarkBlue);
        context.Rectangle(ShapeMode.Stroke, 1, 1, 320, 358, Color.Blue);
        context.DrawString(GlobalResources.Font, "Debug menu", 3, 3);
        context.DrawString(GlobalResources.Font, "FPS graph", 3, 15);
        context.Rectangle(ShapeMode.Fill, 3, 28, 200, 60, new Color(16, 16, 16, 128));
        for (int i = 0; i < FPSValues.Count; i++)
        {
            int value = FPSValues[i];
            float percentFPS = (float)value / EngineCore.Game.Window.CurrentDisplay.DesktopMode.RefreshRate;
            int y = 88 - (int)(60 * (percentFPS >= 1 ? 1 : percentFPS));
            context.Line(3 + i, y, 3 + i, 88, Color.LightGreen);
            context.Pixel(3 + i, y, Color.Green);
        }

        if (EngineCore.CurrentMap != null)
        {
            int objCount = 0;
            foreach (var layer in EngineCore.CurrentMap.Layers)
            {
                objCount += layer.Objects.Count;
            }
            context.DrawString(GlobalResources.Font, $"Current map: {EngineCore.CurrentMap.Filename}\n{objCount} objects", 3, 90);
        }
    }

    public static void Update(float delta)
    {
        FPSValues.Add((int)Chroma.Diagnostics.PerformanceCounter.FPS);
        int[] result = new int[FPSValues.Count - 1];
        Array.Copy(FPSValues.ToArray(), 1, result, 0, result.Length);
        FPSValues = result.ToList();
    }
}