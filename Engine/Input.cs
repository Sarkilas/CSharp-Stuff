using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Ingenia;

namespace Ingenia.Engine
{
    /// <summary>
    /// The input module. Handles all input specific data and functions.
    /// </summary>
    public static class Input
    {
        // Input specific variables
        static KeyboardState keyState;
        static MouseState mouseState;
        public static Keys[] pressedKeys;
        static List<Keys> triggerKeys = new List<Keys>();
        public static bool LeftMouse = false;
        public static bool RightMouse = false;

        // Gamepad specific variables
        static GamePadState padState;
        static List<Buttons> padTriggers = new List<Buttons>();

        // Update the module
        public static void Update()
        {
            // Set new states
            keyState = Keyboard.GetState();
            MouseState mState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if(padState == null) padState = GamePad.GetState(PlayerIndex.One);
            GamePadState pState = GamePad.GetState(PlayerIndex.One);

            // Clear triggers
            triggerKeys.Clear();
            padTriggers.Clear();

            // For each key: add if not included in triggers
            foreach (Keys key in keyState.GetPressedKeys())
            {
                // Unless pressed keys include this key: add to triggers
                if (!Pressed(key)) triggerKeys.Add(key);
            }

            // Fix all triggers
            if (pState.IsButtonDown(Buttons.A) && !padState.IsButtonDown(Buttons.A))
                padTriggers.Add(Buttons.A);
            if (pState.IsButtonDown(Buttons.X) && !padState.IsButtonDown(Buttons.X))
                padTriggers.Add(Buttons.X);
            if (pState.IsButtonDown(Buttons.Y) && !padState.IsButtonDown(Buttons.Y))
                padTriggers.Add(Buttons.Y);
            if (pState.IsButtonDown(Buttons.B) && !padState.IsButtonDown(Buttons.B))
                padTriggers.Add(Buttons.B);
            if (pState.IsButtonDown(Buttons.LeftShoulder) && !padState.IsButtonDown(Buttons.LeftShoulder))
                padTriggers.Add(Buttons.LeftShoulder);
            if (pState.IsButtonDown(Buttons.RightShoulder) && !padState.IsButtonDown(Buttons.RightShoulder))
                padTriggers.Add(Buttons.RightShoulder);
            if (pState.IsButtonDown(Buttons.Start) && !padState.IsButtonDown(Buttons.Start))
                padTriggers.Add(Buttons.Start);
            if (pState.IsButtonDown(Buttons.Back) && !padState.IsButtonDown(Buttons.Back))
                padTriggers.Add(Buttons.Back);

            // Reset mouse presses
            LeftMouse = RightMouse = false;

            // Check for mouse button presses
            if (mState.LeftButton == ButtonState.Pressed &&
                mouseState.LeftButton == ButtonState.Released) LeftMouse = true;
            if (mState.RightButton == ButtonState.Pressed &&
                mouseState.RightButton == ButtonState.Released) RightMouse = true;

            // Set new pressed array
            pressedKeys = keyState.GetPressedKeys();

            // Set new mouse state
            mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            // Set new gamepad state
            padState = GamePad.GetState(PlayerIndex.One);
        }

        // Check if key pressed (continuously held down)
        public static bool Pressed(Keys key)
        {
            bool check = false;

            // Return false if key array is null (not initialized)
            if (pressedKeys == null) return check;

            // Check each pressed key
            foreach (Keys k in pressedKeys)
            {
                if (k.Equals(key))
                {
                    check = true;
                    break;
                }
            }

            return check;
        }

        // Check if triggered
        public static bool Trigger(Keys key)
        {
            return triggerKeys.Contains(key);
        }

        /// <summary>
        /// Gets the gamepad state.
        /// </summary>
        public static GamePadState GamePadData { get { return padState; } }

        /// <summary>
        /// Checks if a gamepad button is triggered (pressed, but not continuously held down)
        /// </summary>
        /// <param name="button">The button to check for.</param>
        /// <returns>Returns true if triggered, false if not.</returns>
        public static bool PadTrigger(Buttons button)
        {
            return padTriggers.Contains(button);
        }

        // Get keyboard state
        public static KeyboardState KeyState
        {
            get { return keyState; }
        }

        // Get mouse state
        public static MouseState Mouse
        {
            get { return mouseState; }
        }

        public static void DrawMouse(SpriteBatch spriteBatch)
        {
            // Get cursor graphic
            Texture2D graphic = Game.Cursor; Color color = Color.White;

            // Draw it on the screen
            spriteBatch.Draw(graphic, new Vector2(Mouse.X - 1, Mouse.Y - 1), color);
        }

        public static bool MouseOver(Rectangle rect)
        {
            return new Rectangle(Mouse.X, Mouse.Y, 1, 1).Intersects(rect);
        }

        public static Keys? GetFirstKey()
        {
            // Create key
            Keys? key = null;

            // Check for first key trigger
            if (triggerKeys.Count > 0) {
                foreach (Keys k in triggerKeys) {
                    if (k != Keys.LeftControl &&
                        k != Keys.RightControl &&
                        k != Keys.LeftAlt &&
                        k != Keys.RightAlt &&
                        k != Keys.LeftShift &&
                        k != Keys.RightShift &&
                        !Database.settings.NoBindKey(k))
                        key = k;
                }
            }

            // Return the key
            return key;
        }

        public static char? GetKeyText()
        {
            if (triggerKeys.Count == 0) return null;
            return GetStringKey(triggerKeys[triggerKeys.Count - 1]);
        }

        public static char? GetStringKey(Keys key, bool noshift = false)
        {
            bool shift = Pressed(Keys.LeftShift) || Pressed(Keys.RightShift);
            if (noshift) shift = false;
            switch (key)
            {
            case Keys.A:
                    return shift ? 'A' : 'a';
 
            case Keys.B:
                    return shift ? 'B' : 'b';
 
            case Keys.C:
                    return shift ? 'C' : 'c';
 
            case Keys.D:
                    return shift ? 'D' : 'd';
 
            case Keys.E:
                    return shift ? 'E' : 'e';
 
            case Keys.F:
                    return shift ? 'F' : 'f';
 
            case Keys.G:
                    return shift ? 'G' : 'g';
 
            case Keys.H:
                    return shift ? 'H' : 'h';
 
            case Keys.I:
                    return shift ? 'I' : 'i';
 
            case Keys.J:
                    return shift ? 'J' : 'j';
 
            case Keys.K:
                    return shift ? 'K' : 'k';
 
            case Keys.L:
                    return shift ? 'L' : 'l';
 
            case Keys.M:
                    return shift ? 'M' : 'm';
 
            case Keys.N:
                    return shift ? 'N' : 'n';
 
            case Keys.O:
                    return shift ? 'O' : 'o';
 
            case Keys.P:
                    return shift ? 'P' : 'p';
 
            case Keys.Q:
                    return shift ? 'Q' : 'q';
 
            case Keys.R:
                    return shift ? 'R' : 'r';
 
            case Keys.S:
                    return shift ? 'S' : 's';
 
            case Keys.T:
                    return shift ? 'T' : 't';
 
            case Keys.U:
                    return shift ? 'U' : 'u';
 
            case Keys.V:
                    return shift ? 'V' : 'v';
 
            case Keys.W:
                    return shift ? 'W' : 'w';
 
            case Keys.X:
                    return shift ? 'X' : 'x';
 
            case Keys.Y:
                    return shift ? 'Y' : 'y';
 
            case Keys.Z:
                    return shift ? 'Z' : 'z';
 
            case Keys.D0:
                    return shift ? ')' : '0';
 
            case Keys.D1:
                    return shift ? '!' : '1';
 
            case Keys.D2:
                    return shift ? '@' : '2';
 
            case Keys.D3:
                    return shift ? '#' : '3';
 
            case Keys.D4:
                    return shift ? '$' : '4';
 
            case Keys.D5:
                    return shift ? '%' : '5';
 
            case Keys.D6:
                    return shift ? '^' : '6';
 
            case Keys.D7:
                    return shift ? '&' : '7';
 
            case Keys.D8:
                    return shift ? '*' : '8';
 
            case Keys.D9:
                    return shift ? '(' : '9';
 
            case Keys.NumPad0:
                    return '0';
 
            case Keys.NumPad1:
                    return '1';
 
            case Keys.NumPad2:
                    return '2';
 
            case Keys.NumPad3:
                    return '3';
 
            case Keys.NumPad4:
                    return '4';
 
            case Keys.NumPad5:
                    return '5';
 
            case Keys.NumPad6:
                    return '6';
 
            case Keys.NumPad7:
                    return '7';
 
            case Keys.NumPad8:
                    return '8';
 
            case Keys.NumPad9:
                    return '9';
 
            case Keys.Decimal:
                    return '.';
 
            case Keys.OemPeriod:
                    return shift ? '>' : '.';
 
            case Keys.OemComma:
                    return shift ? '<' : ',';
 
            case Keys.Space:
                    return ' ';
 
            case Keys.Add:
                    return '+';
 
            case Keys.Divide:
                    return '/';
 
            case Keys.Multiply:
                    return '*';
 
            case Keys.Subtract:
                    return '-';
 
            case Keys.OemPlus:
                    return shift ? '+' : '=';
 
            case Keys.OemMinus:
                    return shift ?  '-' : '_';
 
            case Keys.OemBackslash:                                 // I don't have a keyboard that produces this (the g15), can someone help?
                    return '\\';
 
            case Keys.OemOpenBrackets:
                    return shift ? '{' : '[';
 
            case Keys.OemCloseBrackets:
                    return shift ? '}' : ']';
 
            case Keys.Oem8:                                                 // I don't have a keyboard that produces this (the g15), can someone help?
                    return '8';
 
            case Keys.OemPipe:
                    return shift ? '|' : '\\';
 
            case Keys.OemQuestion:
                    return shift ? '?' : '/';
 
            case Keys.OemQuotes:
                    return shift ? '"' : '\'';
 
            case Keys.OemTilde:
                    return shift ? '~' : '`';
 
            //case Keys.Tab:                                                // Not handled in the sprite font
            //      return '\t';
 
            default:
                    return null;
            }
        }
    }
}
