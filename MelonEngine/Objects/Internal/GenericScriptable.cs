using System.Numerics;

namespace MelonEngine.Objects;

/* Generic object but with some script attached */
public class GenericScriptable : GameObject
{
    public bool Internal { get; } = true;
    public override void OnTriggerEnter()
    {
        //TODO: Check for script attached and execute it
    }

    public GenericScriptable(Vector2 position) : base(position)
    {
    }
}