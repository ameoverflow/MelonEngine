using System.Numerics;
using Chroma.Graphics;
using Chroma.Input;
using Chroma.Input.GameControllers;
using TinyTween;
using Color = Chroma.Graphics.Color;

namespace MelonEngine;

public class Menu : Scene
{
    private int selectedOption;
    private int _visibleSubMenu;
    private Tween<float> _transitionTween = new FloatTween();
    private Tween<Color> _backgroundGradientTween = new ColorTween();
    private Tween<Color> _submenuBackgroundGradientTween = new ColorTween();
    private Tween<Color> _promptTween = new ColorTween();

    List<int> widths = new List<int>()
    {
        GlobalResources.BigFont.Measure(I18N.GetString("menu.new_game")).Width,
        GlobalResources.BigFont.Measure(I18N.GetString("menu.continue")).Width,
        GlobalResources.BigFont.Measure(I18N.GetString("menu.custom")).Width,
        GlobalResources.BigFont.Measure(I18N.GetString("menu.options")).Width,
        GlobalResources.BigFont.Measure(I18N.GetString("menu.quit")).Width,
    };
    public void Draw(RenderContext context)
    {
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 18; y++)
            {
                context.DrawTexture(GlobalResources.MenuBackground, new Vector2(x*20, y*20), Vector2.One*2);
            }
        }
        context.Rectangle(ShapeMode.Stroke, 5, 235, widths.Max(r => r) + 12, 112, Color.Black);
        context.Rectangle(ShapeMode.Stroke, 4, 234, widths.Max(r => r) + 12, 112, Color.White);
        context.Rectangle(ShapeMode.Fill, 5, 235, widths.Max(r => r) + 10, 110, _backgroundGradientTween.CurrentValue);
        Text.RenderTextWithShadow(context, GlobalResources.BigFont, I18N.GetString("menu.new_game"), 10, 240, selectedOption == 0 ? _promptTween.CurrentValue : Color.White);
        Text.RenderTextWithShadow(context, GlobalResources.BigFont, I18N.GetString("menu.continue"), 10, 260, selectedOption == 1 ? _promptTween.CurrentValue : Color.White);
        Text.RenderTextWithShadow(context, GlobalResources.BigFont, I18N.GetString("menu.custom"), 10, 280, selectedOption == 2 ? _promptTween.CurrentValue : Color.White);
        Text.RenderTextWithShadow(context, GlobalResources.BigFont, I18N.GetString("menu.options"), 10, 300, selectedOption == 3 ? _promptTween.CurrentValue : Color.White);
        Text.RenderTextWithShadow(context, GlobalResources.BigFont, I18N.GetString("menu.quit"), 10, 320, selectedOption == 4 ? _promptTween.CurrentValue : Color.White);

        if (_visibleSubMenu == 1)
        {
            context.Rectangle(ShapeMode.Stroke, 21, 21, 600, 320, Color.Black);
            context.Rectangle(ShapeMode.Stroke, 19, 19, 602, 322, Color.White);
            context.Rectangle(ShapeMode.Fill, 20, 20, 600, 320, _submenuBackgroundGradientTween.CurrentValue);

            Text.RenderTextWithShadow(context, GlobalResources.BigFont, I18N.GetString("menu.options"), 30, 30, Color.White);
        }

        if (ControlEx.LastControllerUsed == ControllerType.PSGamepad || ControlEx.LastControllerUsed == ControllerType.XboxGamepad)
        {
            context.Rectangle(ShapeMode.Fill, 0, 0, 640, 27, new Color(16, 16, 16, 128));
            context.DrawTexture((ControlEx.LastControllerUsed == ControllerType.PSGamepad ? GlobalResources.PSPrompts : GlobalResources.XboxPrompts)["a"], new Vector2(5, 5), new Vector2(0.2f, 0.2f));
            Text.RenderTextWithShadow(context, GlobalResources.Font, "Accept", 27, 9, Color.White);

            context.DrawTexture((ControlEx.LastControllerUsed == ControllerType.PSGamepad ? GlobalResources.PSPrompts : GlobalResources.XboxPrompts)["b"], new Vector2(90, 5), new Vector2(0.2f, 0.2f));
            Text.RenderTextWithShadow(context, GlobalResources.Font, "Cancel", 112, 9, Color.White);

            context.DrawTexture((ControlEx.LastControllerUsed == ControllerType.PSGamepad ? GlobalResources.PSPrompts : GlobalResources.XboxPrompts)["left"], new Vector2(175, 5), new Vector2(0.2f, 0.2f));
            context.DrawTexture((ControlEx.LastControllerUsed == ControllerType.PSGamepad ? GlobalResources.PSPrompts : GlobalResources.XboxPrompts)["up"], new Vector2(197, 5), new Vector2(0.2f, 0.2f));
            context.DrawTexture((ControlEx.LastControllerUsed == ControllerType.PSGamepad ? GlobalResources.PSPrompts : GlobalResources.XboxPrompts)["down"], new Vector2(219, 5), new Vector2(0.2f, 0.2f));
            context.DrawTexture((ControlEx.LastControllerUsed == ControllerType.PSGamepad ? GlobalResources.PSPrompts : GlobalResources.XboxPrompts)["right"], new Vector2(241, 5), new Vector2(0.2f, 0.2f));
            Text.RenderTextWithShadow(context, GlobalResources.Font, "Select", 265, 9, Color.White);
        }

        context.Rectangle(ShapeMode.Fill, 0, 0, 640, 480, new Color(16, 16, 16, (byte)Math.Round(_transitionTween.CurrentValue)));
    }

    public void Update(float delta)
    {
        _promptTween.Update(delta);
        _backgroundGradientTween.Update(delta);
        _submenuBackgroundGradientTween.Update(delta);
        _transitionTween.Update(delta);
        if ((ControlEx.IsKeyDown(KeyCode.Down) || ControlEx.IsButtonDown(ControllerButton.DpadDown)) && selectedOption < 4 && _visibleSubMenu == 0 && _transitionTween.State == TweenState.Stopped)
        {
            selectedOption++;
            //GlobalResources.MenuTick.PlayInstance();
            _promptTween.Start(Color.Blue, Color.LightBlue, 0.5f, ScaleFuncs.Linear);
        }
        if ((ControlEx.IsKeyDown(KeyCode.Up) || ControlEx.IsButtonDown(ControllerButton.DpadUp)) && selectedOption > 0 && _visibleSubMenu == 0 && _transitionTween.State == TweenState.Stopped)
        {
            selectedOption--;
            //GlobalResources.MenuTick.PlayInstance();
            _promptTween.Start(Color.Blue, Color.LightBlue, 0.5f, ScaleFuncs.Linear);
        }
        if ((ControlEx.IsKeyDown(KeyCode.Escape) || ControlEx.IsButtonDown(ControllerButton.B)) && _visibleSubMenu == 0 && _transitionTween.State == TweenState.Stopped)
        {
            //GlobalResources.MenuSelect.PlayInstance();
            SceneManager.ChangeScene("intro", true);
        }

        if ((ControlEx.IsKeyDown(KeyCode.Escape) || ControlEx.IsButtonDown(ControllerButton.B)) && _visibleSubMenu > 0 && _transitionTween.State == TweenState.Stopped)
        {
            _visibleSubMenu = 0;
            _backgroundGradientTween.Start(new Color(109, 0, 62), new Color(0, 96, 107), 15, ScaleFuncs.Linear);
            _promptTween.Start(Color.Blue, Color.LightBlue, 0.5f, ScaleFuncs.Linear);
            return;
        }

        if ((ControlEx.IsKeyDown(KeyCode.Return) || ControlEx.IsButtonDown(ControllerButton.A)) && _visibleSubMenu == 0 && _transitionTween.State == TweenState.Stopped)
        {
            //GlobalResources.MenuSelect.PlayInstance();
            switch (selectedOption)
            {
                case 0:
                    _transitionTween.RepeatMode = RepeatMode.Disabled;
                    _transitionTween.Start(0,255, 3f, ScaleFuncs.SineEaseIn);
                    _submenuBackgroundGradientTween.Stop(StopBehavior.AsIs);
                    _backgroundGradientTween.Stop(StopBehavior.AsIs);
                    _promptTween.Stop(StopBehavior.AsIs);
                    _transitionTween.OnComplete = () =>
                    {
                        EngineCore.CurrentMap = new Map();
                        MapParser.PreloadMap(SystemVariables.Chapters[0].Filename);
                        SceneManager.ChangeScene("mapintro");
                    };
                    break;
                case 3:
                    _visibleSubMenu = 1;
                    _backgroundGradientTween.Stop(StopBehavior.AsIs);
                    _promptTween.Stop(StopBehavior.AsIs);
                    _submenuBackgroundGradientTween.Start(new Color(109, 0, 62), new Color(0, 96, 107), 15, ScaleFuncs.Linear);
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
            }
        }
    }

    public void FixedUpdate(float delta)
    {
        
    }

    public void Init()
    {
        _promptTween = new ColorTween();
        _backgroundGradientTween = new ColorTween();
        selectedOption = 0;
        _backgroundGradientTween.RepeatMode = RepeatMode.Reversed;
        _backgroundGradientTween.Start(new Color(109, 0, 62), new Color(0, 96, 107), 15, ScaleFuncs.Linear);
        _submenuBackgroundGradientTween.RepeatMode = RepeatMode.Reversed;
        _promptTween.RepeatMode = RepeatMode.Reversed;
        _promptTween.Start(Color.Blue, Color.LightBlue, 0.5f, ScaleFuncs.Linear);
    }
}
