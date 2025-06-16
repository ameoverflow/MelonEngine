using System.Drawing;
using System.Numerics;
using Chroma.Graphics;
using Color = Chroma.Graphics.Color;

namespace MelonEngine;

public class MapIntro : Scene
{
    private RenderTarget _target;

    private float SceneTimer = 0;
    public void Draw(RenderContext context)
    {
        context.Clear(new Color(16, 16, 16));

        Text.RenderTextWithShadow(context, GlobalResources.BigFont, EngineCore.CurrentMap.Title, 10, 320, Color.White);
    }

    public void Update(float delta)
    {
        SceneTimer += delta;
        if (SceneTimer >= 5f)
        {
            SceneManager.ChangeScene("map");
        }
    }

    public void Init()
    {
        MapParser.LoadCurrentMap();
        SceneTimer = 0;
    }

    public void FixedUpdate(float delta)
    {

    }
}
