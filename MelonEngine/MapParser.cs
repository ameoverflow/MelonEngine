using System.Drawing;
using Chroma;
using Chroma.Diagnostics.Logging;
using Chroma.Graphics;
using Chroma.Physics;
using MelonEngine.Objects;
using TiledCSPlus;
using Vector2 = System.Numerics.Vector2;

/*
 * some trivia
 * max map size is 4,294,967,295 x 4,294,967,295 tiles due to tiled limitations
 * or in chunks, it's 67,108,863 x 119,304,647 chunks
 */

namespace MelonEngine;

public static class MapParser
{
    private static Log Log { get; } = LogManager.GetForCurrentAssembly();

    public static void PreloadMap(string path)
    {
        MemoryStream stream = new MemoryStream(EngineCore.Game.Content.Read(path));
        EngineCore.CurrentMap = new Map();
        EngineCore.CurrentMap.TiledMap = new TiledMap(stream);
        //load tilesets
        foreach (var ts in EngineCore.CurrentMap.TiledMap.Tilesets)
        {
            string tilesetpath = ts.Source.Substring(ts.Source.IndexOf("sprites/", StringComparison.Ordinal));
            byte[] tileset = EngineCore.Game.Content.Read(tilesetpath);
            MemoryStream ms = new MemoryStream(tileset);
            EngineCore.CurrentMap.Tilesets.Add(ts.FirstGid, new TiledTileset(ms));
        }

        EngineCore.CurrentMap.Title = EngineCore.CurrentMap.TiledMap.Properties.Single(p => p.Name == "Title").Value;
        EngineCore.CurrentMap.Filename = path;
    }

    private static Texture TileCutter3000(int gid)
    {
        //you know what
        //i moved it to a separate function
        TiledMapTileset mapTileset = EngineCore.CurrentMap.TiledMap.GetTiledMapTileset(gid); // gets which tilemap this tile belongs to
        TiledTileset tileset = EngineCore.CurrentMap.Tilesets[mapTileset.FirstGid]; // gets actual tileset by first id of that map tileset
        var rect = EngineCore.CurrentMap.TiledMap.GetSourceRect(mapTileset, tileset, gid); //get actual tile rect to cut it out of tileset
        //but its not the end lmao
        //now i have to load tileset by tileset source png using chroma and get the correct tile
        Texture tileSetChroma = EngineCore.Game.Content.Load<Texture>("sprites/" + tileset.Image.Source); //load tilemap as a texture
        Texture texture = tileSetChroma.SubTexture(new Rectangle(rect.X, rect.Y, 10, 10)); //cuts out tile from tileset texture
        texture.FilteringMode = TextureFilteringMode.NearestNeighbor;
        return texture;
    }

    public static bool LoadCurrentMap()
    {
        EngineCore.CurrentMap.Layers = new List<MapLayer>(); //empty out currently loaded tiles
        //process each layer back to front

        foreach (var layer in EngineCore.CurrentMap.TiledMap.Layers)
        {
            MapLayer mapLayer = new MapLayer();
            mapLayer.Type = layer.Type;
            if (layer.Type == TiledLayerType.ImageLayer)
            {
                Texture txt = EngineCore.Game.Content.Load<Texture>(layer.Image.Source[(layer.Image.Source.LastIndexOf("../") + 3)..]);
                GameObject image = new Generic(layer.Offset * 2)
                {
                    Sprite = new Sprite(txt) {Position = layer.Offset*2, Scale = new Vector2((layer.Image.Width / txt.Width)*2, (layer.Image.Height / txt.Height)*2)}
                };
                mapLayer.Objects.Add(image);
                EngineCore.CurrentMap.Layers.Add(mapLayer);
            }
            else if (layer.Type == TiledLayerType.ObjectLayer)
            {
                foreach (var obj in layer.Objects)
                {
                    if (obj.Type == TiledObjectType.Eclipse || obj.Type == TiledObjectType.Polygon || obj.Type == TiledObjectType.Polyline)
                    {
                        Log.Error($"Invalid object type: Object ID {obj.Id}");
                        return false;
                    }

                    GameObject e;
                    if (obj.Class != null)
                    {
                        e = (GameObject)Activator.CreateInstance(GameObjectManager.Objects[obj.Class], obj.Position * 2);
                    }
                    else
                    {
                        e = new Generic(obj.Position*2);
                        bool collide = false;
                        try
                        {
                            collide = obj.Properties.Single(p => p.Name == "Collide").Value == "true";
                        }
                        catch (InvalidOperationException)
                        {
                            Log.Warning($"Property 'Collide' not found: Object ID {obj.Id}. Assuming false");
                        }

                        if (collide)
                        {
                            e.AttachCollider(new RectangleCollider(e, (int)obj.Size.Y*2, (int)obj.Size.Y*2));
                        }

                        if (obj.Gid != 0)
                        {
                            Texture txt = TileCutter3000(obj.Gid);
                            e.Sprite = new Sprite(txt) { Position = obj.Position*2, Scale = new Vector2(obj.Size.X/10*2, obj.Size.Y/10*2)};
                        }
                    }
                    mapLayer.Objects.Add(e);

                    Log.Debug($"Parsed object {obj.Position.X.ToString()},{obj.Position.Y.ToString()}");
                }
            }
            else if (layer.Type == TiledLayerType.TileLayer)
            {
                for (var y = 0; y < EngineCore.CurrentMap.TiledMap.Height; y++)
                {
                    for (var x = 0; x < EngineCore.CurrentMap.TiledMap.Width; x++)
                    {
                        int index = (y * EngineCore.CurrentMap.TiledMap.Width) + x; // tile index
                        int gid = layer.Data[index]; // what tile from tile set is this
                        if (gid == 0) continue;
                        int tileX = (x * 20); // tile x
                        int tileY = (y * 20); // tile y
                        GameObject _e = new Generic(new Vector2(tileX, tileY)); // new entity
                        _e.Sprite = new Sprite(TileCutter3000(gid)) { Position = new Vector2(tileX, tileY), Scale = new Vector2(2, 2)};
                        _e.AttachCollider(new RectangleCollider(_e, 20, 20) {Tag = gid.ToString()});
                        mapLayer.Objects.Add(_e);

                        Log.Debug($"Parsed tile {x.ToString()},{y.ToString()}");
                    }
                }
            }
            EngineCore.CurrentMap.Layers.Add(mapLayer);
            Log.Debug($"Parsed layer {layer.Name}");
        }

        return true;
    }
}

public class InternalMapGameObjectException : Exception
{
    public InternalMapGameObjectException(string id, string name) : base(String.Format($"Object type {name} is internal and can't be created. Object ID: {id}"))
    {
    }
}
