using TiledCSPlus;
namespace MelonEngine;

public class MapLayer
{
    public List<GameObject> Objects { get; set; } = new ();
    public TiledLayerType Type { get; set; }
}