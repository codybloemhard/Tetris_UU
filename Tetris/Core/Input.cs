using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
/*Kleine input wrapper. Om te zien of geklikt is dan hoeven we 
dat niet steeds te checken in de gamecode op basis van het vorige frame etc.*/
namespace Core
{
    public enum MouseButton : byte
    {
        LEFT,
        RIGHT,
        MIDDLE,
        BUTTON0,
        BUTTON1
    }

    public enum PressAction : byte
    {
        DOWN,
        UP,
        PRESSED,
        RELEASED
    }

    public static class Input
    {
        private enum Mstate : byte
        {
            PREV,
            CURRENT
        }
        private static MouseState prevState, currentState;

        static Input()
        {
            prevState = Mouse.GetState();
            currentState = Mouse.GetState();
        }

        public static void Update()
        {
            prevState = currentState;
            currentState = Mouse.GetState();
        }

        private static bool GetButton(Mstate state, MouseButton button)
        {
            MouseState s;
            if (state == Mstate.CURRENT) s = currentState;
            else s = prevState;
            switch (button)
            {
                case MouseButton.LEFT:
                    return s.LeftButton == ButtonState.Pressed;
                case MouseButton.RIGHT:
                    return s.RightButton == ButtonState.Pressed;
                case MouseButton.MIDDLE:
                    return s.MiddleButton == ButtonState.Pressed;
                case MouseButton.BUTTON0:
                    return s.XButton1 == ButtonState.Pressed;
                case MouseButton.BUTTON1:
                    return s.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        public static bool GetMouseButton(PressAction action, MouseButton button)
        {
            if (action == PressAction.DOWN)
                return GetButton(Mstate.CURRENT, button);
            else if (action == PressAction.UP)
                return !GetButton(Mstate.CURRENT, button);
            else if (action == PressAction.PRESSED)
                return GetButton(Mstate.CURRENT, button) && !GetButton(Mstate.PREV, button);
            else if (action == PressAction.RELEASED)
                return !GetButton(Mstate.CURRENT, button) && GetButton(Mstate.PREV, button);
            return false;
        }

        public static Vector2 GetMousePosition()
        {
            return Grid.ToGridSpace(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
        }
    }
}