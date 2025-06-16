using System.Numerics;
using Chroma.Graphics;

namespace MelonEngine;

public class TestObject : GameObject
{
    public TestObject(Vector2 position)
        : base(position)
    {
        this.Sprite = new Sprite(EngineCore.Game.Content.Load<Texture>("editor/cursor.png", Array.Empty<object>()))
        {
            Position = position
        };
    }

    public override void OnTriggerEnter() => throw new NotImplementedException();
}