using System.Numerics;
using Chroma.Graphics;

namespace MelonEngine;

public class Checkpoint : GameObject
{
    public Checkpoint(Vector2 position)
        : base(position)
    {
        this.Sprite = new Sprite(EngineCore.Game.Content.Load<Texture>("sprites/checkpoint.png"))
        {
            Position = position,
            Scale = Vector2.One * 2
        };
    }

    public override void OnTriggerEnter() => throw new NotImplementedException();
}