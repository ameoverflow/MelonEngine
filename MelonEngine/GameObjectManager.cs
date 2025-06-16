using System.Numerics;

namespace MelonEngine;

public static class GameObjectManager
{
    public static Dictionary<string, Type> Objects { get; private set; } = new();
    public static void RegisterObject(this Type objectType, string id)
    {
        GameObject instance = (GameObject) Activator.CreateInstance(objectType, (object) Vector2.Zero);
        if (instance.Internal)
            throw new InternalGameObjectException(((object) instance).GetType().Name);
        Objects.Add(id, objectType);
    }
}

public class InternalGameObjectException : Exception
{
    public InternalGameObjectException(string type)
        : base(string.Format("Object type " + type + " is internal and can't be registered"))
    {

    }
}