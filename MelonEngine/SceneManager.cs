using Chroma.Graphics;

namespace MelonEngine
{
    public static class SceneManager
    {
        private static bool _currentSceneInitialized;
        public static Dictionary<string, Scene> Scenes = new Dictionary<string, Scene>()
        {
            { "intro", new Intro() },
            { "menu", new Menu() },
            { "map", new MapScene() },
            { "mapintro", new MapIntro()}
        };
        public static Scene _currentScene;

        public static void ChangeScene(string id, bool dontReinitalizeScene = false)
        {
            if (Scenes.ContainsKey(id))
            {
                if (!dontReinitalizeScene)
                    _currentSceneInitialized = false;
                _currentScene = Scenes[id];
            }
            else
            {
                throw new SceneNotFoundException(id);
            }
        }

        public static void FixedUpdate(float delta)
        {
            _currentScene.FixedUpdate(delta);
        }

        public static void Update(float delta)
        {
            if (!_currentSceneInitialized)
            {
                _currentScene.Init();
                _currentSceneInitialized = true;
            }
            _currentScene.Update(delta);
        }

        public static void Draw(RenderContext context)
        {
            //don't try to draw the scene before it's initialized you stupid fuck
            if (_currentSceneInitialized)
                _currentScene.Draw(context);
        }
    }

    public class SceneNotFoundException : Exception
    {
        public SceneNotFoundException(string scene)
            : base($"Scene {scene} cannot be found")
        {

        }
    }
}