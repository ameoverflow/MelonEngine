using System.Numerics;
using Chroma;

namespace MelonEngine;

public abstract class GameObject : Entity
{
    public GameObject(Vector2 position)
    {
        Position = position;
    }

    /// <summary>
    /// Executes when player enters this object
    /// </summary>
    public virtual void OnTriggerEnter()
    {

    }

    /// <summary>
    /// Set this to true if this object type should not be allowed to be created in the editor, only by engine
    /// </summary>
    public virtual bool Internal { get; private set; }
}