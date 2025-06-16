using System.Numerics;
using Chroma;
using Chroma.Audio.Sources;
using Chroma.Diagnostics.Logging;
using Chroma.Graphics;
using Chroma.Input;
using Chroma.Physics;
using TiledCSPlus;
using TinyTween;

namespace MelonEngine;

public class MapScene : Scene
{
    private Player _player;
    private float _playerVelocity;
    private Music _bgMusic;
    private DateTime _mapLoadingStarted, _mapLoadingFinished;
    public static Log Log { get; } = LogManager.GetForCurrentAssembly();
    private static Camera _cam;
    private Tween<float> _introTransitionTween = new FloatTween();
    public void Draw(RenderContext context)
    {
        context.Clear(Color.Black);
        context.WithCamera(_cam, (renderContext, camera) =>
        {
            var cam = camera;
            foreach (MapLayer layer in EngineCore.CurrentMap.Layers)
            {
                Entity tempCam = new Entity()
                {
                    Position = new Vector2(cam.Position.X, cam.Position.Y)
                };
                tempCam.AttachCollider(new RectangleCollider(tempCam, (int)(cam.Position.X + (640 / cam.ZoomX)), (int)(cam.Position.Y + (640 / cam.ZoomY))));
                if (layer.Type == TiledLayerType.TileLayer)
                {
                    foreach (Entity entity in layer.Objects)
                    {
                        entity.Sprite.Draw(context);
                    }
                }
                else if (layer.Type == TiledLayerType.ObjectLayer)
                {
                    foreach (Entity entity in layer.Objects)
                    {
                        entity.Sprite.Draw(context);
                    }
                }
                else if (layer.Type == TiledLayerType.ImageLayer)
                {
                    // get bounding box for image layer to calculate if it's in viewing distance
                    Entity obj = layer.Objects[0];
                    layer.Objects[0].Sprite.Draw(context);
                    layer.Objects[0].Draw(context);
                }

                tempCam.DetachCollider();
            }
            _player.Sprite.Draw(context);
            _player.Draw(context);
        });
        context.Rectangle(ShapeMode.Fill, 0, 0, 640, 480, new Color(16, 16, 16, (byte)Math.Round(_introTransitionTween.CurrentValue)));
        context.DrawString(GlobalResources.Font, "Chapter 1", 10, 310, new Color(255, 255, 255, (byte)Math.Round(_introTransitionTween.CurrentValue)));
        context.DrawString(GlobalResources.BigFont, EngineCore.CurrentMap.Title, 10, 320, new Color(255, 255, 255, (byte)Math.Round(_introTransitionTween.CurrentValue)));
    }

    public void Update(float delta)
    {
        _player.Update(delta);
        _player.Sprite.Position = new Vector2(_player.Position.X - 10, _player.Position.Y - 10);
        if (_introTransitionTween.State == TweenState.Running)
        {
            _introTransitionTween.Update(delta);
        }
    }

    public void FixedUpdate(float delta)
    {
        if (_introTransitionTween.State == TweenState.Stopped)
        {
            if (Keyboard.IsKeyDown(KeyCode.Right))
            {
                _cam.Position += new Vector3(2, 0, 0);
            }

            if (Keyboard.IsKeyDown(KeyCode.Left))
            {
                _cam.Position += new Vector3(-2, 0, 0);
            }

            if (ControlEx.IsKeyDown(KeyCode.Up))
            {
                _cam.Position += new Vector3(0, 1, 0);
            }
        }
    }

    public void Init()
    {
        _introTransitionTween = new FloatTween();
        _introTransitionTween.RepeatMode = RepeatMode.Disabled;
        _introTransitionTween.Start(255, 0, 1f, ScaleFuncs.SineEaseIn);
        _playerVelocity = 0;
        _cam = new Camera(0, 0)
        {
            UseCenteredOrigin = true
        };

        //_bgMusic = EngineCore.Game.Content.Load<Music>("audio/music/" + _map.AudioTrack);
        //_bgMusic.Play();
        _player = new Player() { Position = new Vector2(10, 300) };
        _player.Sprite = new Sprite(GlobalResources.Player) {Position = _player.Position/2, Scale = new Vector2(2, 2)};
        _player.AttachCollider(new CircleCollider(_player, 10) { Tag = "player" });
        _mapLoadingFinished = DateTime.Now;
    }
}