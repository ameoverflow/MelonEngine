using Chroma.Input;
using Chroma.Input.GameControllers;

namespace MelonEngine;

public static class ControlEx
{
    private static HashSet<KeyCode> _keysDown = new();
    private static HashSet<KeyCode> _keysHold = new();
    
    private static HashSet<ControllerButton> _buttonsDown = new();
    private static HashSet<ControllerButton> _buttonsHold = new();

    public static ControllerType LastControllerUsed { get; private set; } = ControllerType.Keyboard;
    
    public static void Update()
    {
        foreach (var key in Enum.GetValues<KeyCode>())
        {
            if (Keyboard.ActiveKeys.Contains(key) && !_keysHold.Contains(key))
            {
                _keysHold.Add(key);
                _keysDown.Add(key);
            } else if (Keyboard.ActiveKeys.Contains(key) && _keysHold.Contains(key))
            {
                _keysDown.Remove(key);
            } else if (!Keyboard.ActiveKeys.Contains(key) && _keysHold.Contains(key))
            {
                _keysHold.Remove(key);
            }
        }

        if (Controller.DeviceCount != 0)
        {
            foreach (var button in Enum.GetValues<ControllerButton>())
            {
                if (Controller.GetActiveButtons(0).Contains(button) && !_buttonsHold.Contains(button)) 
                {
                    _buttonsHold.Add(button);
                    _buttonsDown.Add(button);
                } else if (Controller.GetActiveButtons(0).Contains(button) && _buttonsHold.Contains(button)) 
                {
                    _buttonsDown.Remove(button);
                } else if (!Controller.GetActiveButtons(0).Contains(button) && _buttonsHold.Contains(button)) 
                {
                    _buttonsHold.Remove(button);
                }
            }
            
            if ((LastControllerUsed == ControllerType.PSGamepad || LastControllerUsed == ControllerType.XboxGamepad) && Keyboard.ActiveKeys.Count > 0 && Controller.GetActiveButtons(0).Count == 0)
                LastControllerUsed = ControllerType.Keyboard;

            if (Controller.DeviceCount == 0)
                LastControllerUsed = ControllerType.Keyboard;

            if (LastControllerUsed == ControllerType.Keyboard && Keyboard.ActiveKeys.Count == 0 &&
                Controller.GetActiveButtons(0).Count > 0)
            {
                if (Controller.Get(0).Info.Name.ToLower().Contains("ps") ||
                    Controller.Get(0).Info.Name.ToLower().Contains("playstation"))
                {
                    LastControllerUsed = ControllerType.PSGamepad;
                }
                else
                {
                    LastControllerUsed = ControllerType.XboxGamepad;
                }
            }
        }
    }

    public static bool IsKeyDown(KeyCode key)
    {
        return _keysDown.Contains(key);
    }
    
    public static bool IsButtonDown(ControllerButton button)
    {
        return _buttonsDown.Contains(button);
    }
}

public enum ControllerType
{
    PSGamepad,
    XboxGamepad,
    Keyboard
}