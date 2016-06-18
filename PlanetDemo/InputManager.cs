using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PlanetDemo
{
    public enum MouseButton
    {
        Left,
        Right, 
        Middle,
        XButton1,
        XButton2
    }
    public sealed class InputManager
    {
        public bool LockMouse = false;

        private static volatile InputManager instance;
        private static object syncRoot = new Object();

        public GraphicsDeviceManager graphics;

        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new InputManager();
                    }
                }
                return instance;
            }
        }

        KeyboardState currentKeyState, prevKeyState;
        MouseState currentMouseState, prevMouseState;

        Vector2 prevCursorPosition;
        public Vector2 CurrentCursorPosition;

        public Vector2 cursorDelta { get { return CurrentCursorPosition - prevCursorPosition; } }
        public Vector2 normalizedCursorDelta { get { return new Vector2(cursorDelta.X / graphics.PreferredBackBufferWidth, cursorDelta.Y / graphics.PreferredBackBufferHeight); } }
        

        public void Update(GameTime gameTime)
        {
            prevKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            if (!LockMouse)
            {
                prevCursorPosition = CurrentCursorPosition;
                CurrentCursorPosition = new Vector2(currentMouseState.X, currentMouseState.Y);
            } else
            {
                Vector2 middle = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

                prevCursorPosition = middle;
                CurrentCursorPosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                try
                {
                    Mouse.SetPosition((int) middle.X, (int)middle.Y);
                }
                catch { }
            }
        }

        public bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
                if (currentKeyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key))
                    return true;
            return false;
        }

        public bool KeyReleased(params Keys[] keys)
        {
            foreach (Keys key in keys)
                if (currentKeyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key))
                    return true;
            return false;
        }

        public bool KeyDown(params Keys[] keys)
        {
            
            foreach (Keys key in keys)
                if (currentKeyState.IsKeyDown(key))
                    return true;
            return false;
        }

        public bool IsCursorWithinRect(Rectangle rect)
        {
            if (rect.Contains(currentMouseState.X, currentMouseState.Y))
                return true;
            return false;
        }

        public bool MousePressed(params MouseButton[] buttons)
        {
            foreach(MouseButton btn in buttons)
            {
                switch (btn)
                {
                    case MouseButton.Left:
                        if (currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
                            return true;
                        break;
                    case MouseButton.Right:
                        if (currentMouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
                            return true;
                        break;
                    case MouseButton.Middle:
                        if (currentMouseState.MiddleButton == ButtonState.Pressed && prevMouseState.MiddleButton == ButtonState.Released)
                            return true;
                        break;
                    case MouseButton.XButton1:
                        if (currentMouseState.XButton1 == ButtonState.Pressed && prevMouseState.XButton1 == ButtonState.Released)
                            return true;
                        break;
                    case MouseButton.XButton2:
                        if (currentMouseState.XButton2 == ButtonState.Pressed && prevMouseState.XButton2 == ButtonState.Released)
                            return true;
                        break;
                }
            }
            return false;
        }
        public bool MouseReleased(params MouseButton[] buttons)
        {
            foreach (MouseButton btn in buttons)
            {
                switch (btn)
                {
                    case MouseButton.Left:
                        if (currentMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
                            return true;
                        break;
                    case MouseButton.Right:
                        if (currentMouseState.RightButton == ButtonState.Released && prevMouseState.RightButton == ButtonState.Pressed)
                            return true;
                        break;
                    case MouseButton.Middle:
                        if (currentMouseState.MiddleButton == ButtonState.Released && prevMouseState.MiddleButton == ButtonState.Pressed)
                            return true;
                        break;
                    case MouseButton.XButton1:
                        if (currentMouseState.XButton1 == ButtonState.Released && prevMouseState.XButton1 == ButtonState.Pressed)
                            return true;
                        break;
                    case MouseButton.XButton2:
                        if (currentMouseState.XButton2 == ButtonState.Released && prevMouseState.XButton2 == ButtonState.Pressed)
                            return true;
                        break;
                }
            }
            return false;
        }
        public bool MouseDown(params MouseButton[] buttons)
        {
            foreach (MouseButton btn in buttons)
            {
                switch (btn)
                {
                    case MouseButton.Left:
                        if (currentMouseState.LeftButton == ButtonState.Pressed)
                            return true;
                        break;
                    case MouseButton.Right:
                        if (currentMouseState.RightButton == ButtonState.Pressed)
                            return true;
                        break;
                    case MouseButton.Middle:
                        if (currentMouseState.MiddleButton == ButtonState.Pressed)
                            return true;
                        break;
                    case MouseButton.XButton1:
                        if (currentMouseState.XButton1 == ButtonState.Pressed)
                            return true;
                        break;
                    case MouseButton.XButton2:
                        if (currentMouseState.XButton2 == ButtonState.Pressed)
                            return true;
                        break;
                }
            }
            return false;
        }

    }
}
