using System.Numerics;
using Chroma.Graphics;

namespace MelonEngine.Objects;

/* Generic object, represents almost everything which is not its own object class:
 Images, tiles, dumb objects (i.e. without class and script)
 Does absolutely nothing and gets cleaned by garbage collector when not needed */
public class Generic : GameObject
{
    public new bool Internal { get; } = true;
    public override void OnTriggerEnter()
    {
        throw new NotImplementedException();
    }

    public Generic(Vector2 position) : base(position)
    {
        Sprite = new Sprite(GlobalResources.Unknown) { Position = position };
    }
}