using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Chroma.Audio;
using Chroma.ContentManagement;
using Chroma.Diagnostics.Logging;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering.TrueType;
using Chroma.Input;
using Chroma.Input.GameControllers;
using Color = Chroma.Graphics.Color;
using Game = Chroma.Game;

namespace MelonEngine;

public class EngineCore : Game
{
    public static Game Game { get; private set; }

    private RenderTarget _secondaryFramebuffer;
    private RenderTarget _mainFramebuffer;
    private static bool _notificationShow;
    private static float _notificationCooldown;
    private static string _notificationText = "";
    private static bool _f3Enabled;
    private static Version? _version = Assembly.GetExecutingAssembly().GetName().Version;
    private static DateTime _buildDate = new DateTime(2000, 1, 1)
        .AddDays(_version.Build).AddSeconds(_version.Revision * 2);
    private Version version1 = Assembly.LoadFrom("chroma.dll").GetName().Version;

    private string _lang = "en_US"; //default to en_US

    public static Log Log { get; } = LogManager.GetForCurrentAssembly();
    public static string[] CommandLine { get; } = Environment.CommandLine.Split(" ");

    internal static AudioOutput AudioOutput { get; set; }
    internal static Map CurrentMap;
    internal static bool DebugMode = Environment.GetCommandLineArgs().Contains("--debug");
    internal static Texture Framebuffer;
    public EngineCore() : base(new(false, false))
    {
        Log.Info("Welcome to Melon Engine");

        Log.Info("Initializing...");
        Log.Debug($"Command line: {Environment.CommandLine}");

        float scale = 2;
        Window.Mode.SetWindowed(new Size((int)Math.Floor(640*scale), (int)Math.Floor(360*scale)), true);
        Window.Title = $"Melon Engine build {Assembly.GetExecutingAssembly().GetName().Version}";
        Graphics.VerticalSyncMode = VerticalSyncMode.None;

        _mainFramebuffer = new RenderTarget(640, 360);
        _mainFramebuffer.FilteringMode = TextureFilteringMode.NearestNeighbor;
        _mainFramebuffer.VirtualResolution = new Size(Window.Width, Window.Height);

        _secondaryFramebuffer = new RenderTarget(640, 360);
        _secondaryFramebuffer.FilteringMode = TextureFilteringMode.NearestNeighbor;
        _secondaryFramebuffer.VirtualResolution = new Size(Window.Width, Window.Height);

        RenderSettings.DefaultTextureFilteringMode = TextureFilteringMode.NearestNeighbor;

        if (RootChecker.IsRoot())
            Log.Warning("You are running the game as root/administrator. Doing this can cause malicious level scripts to damage your system. Administrator permissions are not required to run the game.");

        //parse command line
        if (Environment.GetCommandLineArgs().Contains("--lang") && Environment.GetCommandLineArgs().Length > Array.IndexOf(Environment.GetCommandLineArgs(), "--lang") + 1)
        {
            _lang = Environment.GetCommandLineArgs()[Array.IndexOf(Environment.GetCommandLineArgs(), "--lang") + 1];
        }

        Game = this;
        AudioOutput = Audio.Output;

        Log.Info("Finished engine initialization");
    }

    protected override IContentProvider InitializeContentPipeline()
    {
        var pipeline = new ZipContentProvider(this, Path.Combine(AppContext.BaseDirectory, "data.pk3"));

        pipeline.RegisterImporter<InstancedSound>((path, _) => new InstancedSound(path));

        return pipeline;
    }

    protected override void LoadContent()
    {
        Log.Info("Loading resources");
        
        GlobalResources.Font = Content.Load<TrueTypeFont>("fonts/PixeloidSans.ttf", 9);
        GlobalResources.BigFont = Content.Load<TrueTypeFont>("fonts/PixeloidSans.ttf", 18);
        GlobalResources.Player = Content.Load<Texture>("sprites/player/player.png");
        GlobalResources.ChromaLogo = Content.Load<Texture>("sprites/logos/Chroma.png");
        GlobalResources.MELogo = Content.Load<Texture>("sprites/logos/unknown.png");
        GlobalResources.Warning = Content.Load<Texture>("sprites/icons/warning.png");
        GlobalResources.Error = Content.Load<Texture>("sprites/icons/error.png");
        GlobalResources.Splashes = Content.Read("text/splashes.txt");
        //GlobalResources.MenuSelect = new InstancedSound(Content.Open("sounds/menu-select.mp3"));
        //GlobalResources.MenuTick = new InstancedSound(Content.Open("sounds/menu-select-tick.wav"));

        GlobalResources.XboxPrompts.Add("b", Content.Load<Texture>("controllerprompts/Xbox 360/360_B.png"));
        GlobalResources.XboxPrompts.Add("a", Content.Load<Texture>("controllerprompts/Xbox 360/360_A.png"));
        GlobalResources.XboxPrompts.Add("left", Content.Load<Texture>("controllerprompts/Xbox 360/360_Dpad_Left.png"));
        GlobalResources.XboxPrompts.Add("up", Content.Load<Texture>("controllerprompts/Xbox 360/360_Dpad_Right.png"));
        GlobalResources.XboxPrompts.Add("down", Content.Load<Texture>("controllerprompts/Xbox 360/360_Dpad_Down.png"));
        GlobalResources.XboxPrompts.Add("right", Content.Load<Texture>("controllerprompts/Xbox 360/360_Dpad_Up.png"));
        GlobalResources.XboxPrompts.Add("menu", Content.Load<Texture>("controllerprompts/Xbox 360/360_Start.png"));

        GlobalResources.PSPrompts.Add("b", Content.Load<Texture>("controllerprompts/PS3/PS3_Circle.png"));
        GlobalResources.PSPrompts.Add("a", Content.Load<Texture>("controllerprompts/PS3/PS3_Cross.png"));
        GlobalResources.PSPrompts.Add("left", Content.Load<Texture>("controllerprompts/PS3/PS3_Dpad_Left.png"));
        GlobalResources.PSPrompts.Add("up", Content.Load<Texture>("controllerprompts/PS3/PS3_Dpad_Up.png"));
        GlobalResources.PSPrompts.Add("down", Content.Load<Texture>("controllerprompts/PS3/PS3_Dpad_Down.png"));
        GlobalResources.PSPrompts.Add("right", Content.Load<Texture>("controllerprompts/PS3/PS3_Dpad_Right.png"));
        GlobalResources.PSPrompts.Add("menu", Content.Load<Texture>("controllerprompts/PS3/PS3_Start.png"));

        GlobalResources.MenuBackground = Content.Load<Texture>("sprites/overworld.png");

        I18N.LoadLanguage(Encoding.UTF8.GetString(Content.Read($"lang/{_lang}.lang")));

        typeof(TestObject).RegisterObject("testObject");
        typeof(Checkpoint).RegisterObject("checkpoint");

        Log.Info("Resources loaded, starting game");
        SceneManager.ChangeScene("intro");
    }

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }


    protected override void Update(float delta)
    {
        if (DebugMenu.Enabled)
            DebugMenu.Update(delta);

        ControlEx.Update();

        if (ControlEx.IsKeyDown(KeyCode.Tab))
            DebugMenu.Enabled = !DebugMenu.Enabled;

        if (ControlEx.IsKeyDown(KeyCode.F3))
            _f3Enabled = !_f3Enabled;

        if (ControlEx.IsKeyDown(KeyCode.F2))
        {
            Window.SaveScreenshot(DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss") + ".bmp");
            Log.Info($"Saved screenshot to {DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss")}.bmp");
        }

        if (_notificationShow)
            _notificationCooldown += delta;
        if (_notificationCooldown >= 4f)
            _notificationShow = false;

        SceneManager.Update(delta);
        InstancedSound.Update();
    }

    protected override void FixedUpdate(float delta)
    {
        SceneManager.FixedUpdate(delta);
    }

    protected override void Draw(RenderContext context)
    {
        context.RenderTo(_mainFramebuffer, (ctx, tgt) => {
            SceneManager.Draw(ctx);
        });

        context.RenderTo(_secondaryFramebuffer, (ctx, target) =>
        {
            ctx.Clear(Color.Transparent);
            string displayableVersion = "";
            if (_f3Enabled)
            {
                displayableVersion =
                    $"Melon Engine {SystemVariables.Version} ({_buildDate.ToString("dddd, dd.MM.yyyy HH:mm")})\n" +
                    $"Chroma {version1}, .NET {Environment.Version.ToString()} on {Environment.OSVersion.Platform} {Environment.OSVersion.Version}\n" +
                    $"{Chroma.Diagnostics.PerformanceCounter.FPS} FPS\n" +
                    $"Mem: {(Process.GetCurrentProcess().PrivateMemorySize64 / 1024f / 1024f).ToString("F0")}MB\n";
            }
            else
            {
                displayableVersion =
                    $"Melon Engine {SystemVariables.Version}";
            }

            ctx.DrawString(GlobalResources.Font, displayableVersion, 1, 1, Color.Black);
            ctx.DrawString(GlobalResources.Font, displayableVersion, 0, 0);


            if (_notificationShow)
            {
                Size size = GlobalResources.Font.Measure(_notificationText);
                size.Width += 10;
                size.Height += 10;
                ctx.Rectangle(ShapeMode.Fill, 1, 1, size.Width, size.Height, Color.Black);
                ctx.Rectangle(ShapeMode.Stroke, 1, 1, size.Width, size.Height, Color.White);
                ctx.DrawString(GlobalResources.Font, _notificationText, 5, 5);
            }

            if (DebugMenu.Enabled)
                DebugMenu.Draw(ctx);
        });

        context.DrawTexture(
            _mainFramebuffer,
            Vector2.Zero,
            Vector2.One,
            Vector2.Zero,
            0
        );

        context.DrawTexture(
            _secondaryFramebuffer,
            Vector2.Zero,
            Vector2.One,
            Vector2.Zero,
            0
        );
    }

    public static void ShowNotification(string text)
    {
        _notificationShow = true;
        _notificationCooldown = 0;
        _notificationText = text;
        Log.Info(text);
    }

    protected override void ControllerDisconnected(ControllerEventArgs e)
    {
        ShowNotification($"Controller disconnected");
    }

    protected override void ControllerConnected(ControllerEventArgs e)
    {
        ShowNotification($"Controller connected - {e.Controller.Info.Name}");
    }
}
