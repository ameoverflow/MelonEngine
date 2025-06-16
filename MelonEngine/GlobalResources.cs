using Chroma.Audio.Sources;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering.TrueType;

namespace MelonEngine;

internal static class GlobalResources
{
    //Basic data
    internal static TrueTypeFont Font { get; set; }
    internal static TrueTypeFont BigFont { get; set; }
    internal static Texture Player { get; set; }
    internal static Color Black { get; } = new Color(10, 10, 10);
    internal static Texture Unknown { get; } = new Texture(10, 10, new byte[]
    {
        213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249, 213, 0, 249
    }, PixelFormat.RGB);
    internal static Texture Empty { get; set; }

    //Sounds
    internal static InstancedSound MenuSelect { get; set; }
    internal static InstancedSound MenuTick { get; set; }
    
    //Logos
    internal static Texture ChromaLogo { get; set; }
    internal static Texture MELogo { get; set; }
    
    //Menu Textures
    internal static Texture MenuBackground { get; set; }
    
    //Icons
    internal static Texture Warning { get; set; }
    internal static Texture Error { get; set; }

    internal static Dictionary<string, Texture> PSPrompts = new Dictionary<string, Texture>();
    internal static Dictionary<string, Texture> XboxPrompts = new Dictionary<string, Texture>();
    
    //Raw stuff
    internal static byte[] Splashes { get; set; }
}
