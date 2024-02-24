using System;
using Microsoft.Xna.Framework.Input;

namespace Ingenia.Engine
{
    [Serializable]
    public class Keybinding
    {
        public Keys Key;
        public bool Shift;
        public bool Ctrl;
        public bool Alt;

        public Keybinding(Keys k, bool s = false, bool c = false, bool a = false)
        {
            Key = k;
            Shift = s;
            Ctrl = c;
            Alt = a;
        }
        public Keybinding() { }

        /// <summary>
        /// Checks if this keybinding is pressed (held down).
        /// </summary>
        /// <returns>Returns true if binding is pressed, false if not.</returns>
        public bool Pressed {
            get
            {
                bool check = true;
                if (Shift && !(Input.Pressed(Keys.LeftShift) ||
                    Input.Pressed(Keys.RightShift))) check = false;
                if (Ctrl && (Input.Pressed(Keys.LeftControl) ||
                    Input.Pressed(Keys.RightControl))) check = false;
                if (Alt && (Input.Pressed(Keys.LeftAlt) ||
                    Input.Pressed(Keys.RightAlt))) check = false;
                if (!Shift && (Input.Pressed(Keys.LeftShift) ||
                    Input.Pressed(Keys.RightShift))) check = false;
                if (!Ctrl && (Input.Pressed(Keys.LeftControl) ||
                    Input.Pressed(Keys.RightControl))) check = false;
                if (!Alt && (Input.Pressed(Keys.LeftAlt) ||
                    Input.Pressed(Keys.RightAlt))) check = false;
                if (!Input.Pressed(Key)) check = false;
                return check;
            }
        }

        /// <summary>
        /// Checks if a keybinding is utilizing the same modifiers and key.
        /// </summary>
        /// <param name="obj">The keybinding object to compare with.</param>
        /// <returns>Returns true if equal, false if not.</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Keybinding))
            {
                Keybinding b = (Keybinding)obj;
                return (b.Key == Key &&
                    b.Shift == Shift &&
                    b.Ctrl == Ctrl &&
                    b.Alt == Alt);
            }
            else
                return base.Equals(obj);
        }

        /// <summary>
        /// The key string of this keybinding.
        /// </summary>
        public string KeyString
        {
            get
            {
                string key = "";
                if (Ctrl)
                    key += "Ctrl+";
                if (Shift)
                    key += "Shift+";
                if (Alt)
                    key += "Alt+";
                if (Key.ToString().Substring(0, 1) == "D" && Key.ToString().Length == 2)
                    key += Key.ToString().Substring(1, 1);
                else
                    key += Key.ToString();

                return key;
            }
        }

        public override string ToString()
        {
            string key = "";
            if (Ctrl)
                key += "C";
            if (Shift)
                key += "S";
            if (Alt)
                key += "A";
            if (Key.ToString().Substring(0, 1) == "D" && Key.ToString().Length == 2)
                key += Key.ToString().Substring(1, 1);
            else
                key += Key.ToString();

            return key;
        }
        
        /// <summary>
        /// Checks if this keybinding is triggered (pressed once).
        /// </summary>
        /// <returns>Returns true if binding is triggered, false if not.</returns>
        public bool Triggered {
            get
            {
                bool check = true;
                if (Shift && !(Input.Pressed(Keys.LeftShift) ||
                    Input.Pressed(Keys.RightShift))) check = false;
                if (Ctrl && !(Input.Pressed(Keys.LeftControl) ||
                    Input.Pressed(Keys.RightControl))) check = false;
                if (Alt && !(Input.Pressed(Keys.LeftAlt) ||
                    Input.Pressed(Keys.RightAlt))) check = false;
                if (!Input.Trigger(Key)) check = false;
                return check;
            }
        }
    }
}
