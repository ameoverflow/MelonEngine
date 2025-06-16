using Chroma.Audio.Sources;
using TiledCSPlus;

namespace MelonEngine;

internal class Map
{
    internal Dictionary<int, TiledTileset> Tilesets { get; set; } = new Dictionary<int, TiledTileset>();
    internal string Filename { get; set; }
    internal List<MapLayer> Layers = new List<MapLayer>();
    internal Music AudioTrack { get; set; }
    internal string Title { get; set; }
    internal TiledMap TiledMap { get; set; }
}