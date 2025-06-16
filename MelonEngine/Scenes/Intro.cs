using System.Numerics;
using System.Reflection;
using System.Text;
using Chroma.Graphics;
using Chroma.Input;
using Chroma.Input.GameControllers;
using TinyTween;
using Color = Chroma.Graphics.Color;

namespace MelonEngine;

public class Intro : Scene
{
    private Tween<float> _introTransitionTween = new FloatTween();
    private Tween<Color> _backgroundGradientTween = new ColorTween();
    private Tween<Color> _promptTween = new ColorTween();

    private float SceneTimer = 0;
    private string[] TitleTexts = new[] { "missingno." };
    
    private string TitlteChoice;
    private bool transitionStarted = false;
    public void Draw(RenderContext context)
    {
        context.Clear(new Color(16, 16, 16));
        if (SceneTimer <= 5f)
        {
            context.DrawString(GlobalResources.BigFont, "ameOverflow presents",(640 - GlobalResources.BigFont.Measure("ameOverflow Presents").Width) / 2, (180 - (GlobalResources.BigFont.Measure("ameOverflow Presents").Height) / 2));
            context.DrawString(GlobalResources.Font, TitlteChoice,(640 - GlobalResources.Font.Measure(TitlteChoice).Width) / 2, 30 + (180 - (GlobalResources.Font.Measure(TitlteChoice).Height) / 2));
            context.DrawString(GlobalResources.Font, "Powered by:", 10, 342);
            context.DrawTexture(GlobalResources.ChromaLogo, new Vector2(75, 335), new Vector2(0.2f, 0.2f));
            context.DrawTexture(GlobalResources.MELogo, new Vector2(105, 335), new Vector2(0.05f, 0.05f));
            context.DrawTexture(GlobalResources.Warning, new Vector2(316, 240));
            context.DrawString(GlobalResources.Font, I18N.GetString("splash.warning"),(640 - GlobalResources.Font.Measure(I18N.GetString("splash.warning")).Width) / 2, 255);
        }

        if (SceneTimer >= 5f)
        {
            context.Clear(_backgroundGradientTween.CurrentValue);
            context.DrawString(GlobalResources.BigFont, "untitled", (640 - GlobalResources.BigFont.Measure("untitled").Width) / 2, (360 - GlobalResources.BigFont.Measure("untitled").Height) / 2);
            string startText = ControlEx.LastControllerUsed == ControllerType.Keyboard
                ? I18N.GetString("title.start.keyboard")
                : I18N.GetString("title.start.controller");
            startText = startText.ToUpper();
            context.Rectangle(ShapeMode.Fill, (640 - GlobalResources.Font.Measure(startText).Width) / 2 - 30, 318, GlobalResources.Font.Measure(startText).Width + 60, GlobalResources.Font.Measure(startText).Height + 4, new Color(254, 157, 29, 64));
            context.DrawString(GlobalResources.Font, startText, (640 - GlobalResources.Font.Measure(startText).Width) / 2, 320,
                (data, c, i, vector2) =>
                {
                    data.Color = _promptTween.CurrentValue;
                });
            
            Text.RenderTextWithShadow(context, GlobalResources.Font, Assembly.GetExecutingAssembly().GetName().Version.ToString(2), 0, 350, Color.Gray);
            Text.RenderTextWithShadow(context, GlobalResources.Font, "Copyright ameOverflow 2024. Free to distribute!", 640 - GlobalResources.Font.Measure("Copyright ameOverflow 2024. Free to distribute!").Width, 360 - GlobalResources.Font.Measure("Copyright ameOverflow 2024. Free to distribute!").Height, Color.Gray);
            
            if (!transitionStarted)
            {
                transitionStarted = true;
                _introTransitionTween.Start(255, 0, 1f, ScaleFuncs.SineEaseIn);
            }
            context.Rectangle(ShapeMode.Fill, 0, 0, 640, 480, new Color(16, 16, 16, (byte)Math.Round(_introTransitionTween.CurrentValue)));
        }
    }

    public void Update(float delta)
    {
        SceneTimer += delta;
        _promptTween.Update(delta);
        _introTransitionTween.Update(delta);
        _backgroundGradientTween.Update(delta);
        
        if (SceneTimer >= 6f)
        {
            if (ControlEx.IsKeyDown(KeyCode.Space) || ControlEx.IsButtonDown(ControllerButton.Menu))
            {
                //GlobalResources.MenuSelect.PlayInstance();
                SceneManager.ChangeScene("menu");
            }
        }
    }

    public void FixedUpdate(float delta)
    {

    }

    public void Init()
    {

        _introTransitionTween = new FloatTween();
        _backgroundGradientTween = new ColorTween();
        _promptTween = new ColorTween();

        SceneTimer = 0;
        TitleTexts = new[] { "missingno." };

        TitlteChoice = "";
        transitionStarted = false;
        try
        {
            TitleTexts = Encoding.UTF8.GetString(GlobalResources.Splashes).Split("\n");
        }
        catch
        {
        }

        Random _rnd = new Random(DateTime.UtcNow.Millisecond);
        _introTransitionTween.RepeatMode = RepeatMode.Disabled;
        _promptTween.RepeatMode = RepeatMode.Reversed;
        _promptTween.Start(new Color(100, 100, 100), new Color(255, 255, 255), 1f, ScaleFuncs.Linear);
        _backgroundGradientTween.RepeatMode = RepeatMode.Reversed;
        _backgroundGradientTween.Start(new Color(109, 0, 62), new Color(0, 96, 107), 60, ScaleFuncs.Linear);
        TitlteChoice = TitleTexts[_rnd.Next(0, TitleTexts.Length)];
    }
}
