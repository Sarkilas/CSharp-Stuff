using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ingenia.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ingenia.Interface
{
    public class SettingsComponent : Component
    {
        /// <summary>
        /// The settings window.
        /// </summary>
        Window SettingsWindow;

        /// <summary>
        /// The binding window.
        /// </summary>
        Window BindWindow;

        /// <summary>
        /// Tooltips.
        /// </summary>
        List<Window> Tooltips = new List<Window>();

        /// <summary>
        /// The binding keys in order.
        /// </summary>
        string[] BindKeys = new string[] { "upkey", "leftkey", "downkey", "rightkey", "pulse", "sound", "interact", "data" };

        /// <summary>
        /// The binding index.
        /// </summary>
        int BindingIndex = 0;

        public SettingsComponent()
        {
            // Set up the settings window
            SettingsWindow = new Window("Settings", new Rectangle((Screen.Width - 640) / 2,
                (Screen.Height - 334) / 2, 640, 334), new Color(110, 69, 46));

            // Draw
            RedrawSettings();

            #region Add all tooltips
            Tooltips.Add(Window.Dialog(new Color(190, 99, 66), Database.TooltipFont, "May reduce mouse lag"));
            Tooltips.Add(Window.Dialog(new Color(190, 99, 66), Database.TooltipFont, "Only supports XBOX and XUSB gamepads."));
            Tooltips.Add(Window.Dialog(new Color(190, 99, 66), Database.TooltipFont, "Enables hints to show during new game occurences."));
            Tooltips.Add(Window.Dialog(new Color(190, 99, 66), Database.TooltipFont, "Makes the screen edges glow in certain situations."));
            #endregion

            // Remove dialog flags from all tooltips
            foreach (Window tooltip in Tooltips)
            {
                tooltip.DialogFlag = false;
                tooltip.Active = false;
            }

            // Set flag
            SettingsWindow.DialogFlag = true;

            // Initialize the binding window
            BindWindow = Window.Dialog(new Color(110, 69, 46), null, "Press the new key combination.\nYou may utilize Ctrl, Shift and Alt modifiers.", "cancel");
            BindWindow.Active = false;
        }

        public override void Update(GameTime gameTime, Vector2 relative)
        {
            // Update windows
            Button b;
            if (BindWindow.Active)
            {
                // Update binding window
                BindWindow.Update(gameTime);

                // If cancel button clicked: close
                b = (Button)BindWindow.GetComponent("0");
                if (b.Clicked)
                {
                    Audio.PlaySound("decision.ogg");
                    BindWindow.Active = false;
                }

                // Check if a key has been pressed
                Keys? key = Input.GetFirstKey();
                if (key.HasValue)
                {
                    Database.settings.Bind(new Keybinding(key.Value,
                        Input.Pressed(Keys.LeftShift) || Input.Pressed(Keys.RightShift),
                        Input.Pressed(Keys.LeftControl) || Input.Pressed(Keys.RightControl),
                        Input.Pressed(Keys.LeftAlt) || Input.Pressed(Keys.RightAlt)), BindingIndex);
                    RedrawKeys();
                    BindWindow.Active = false;
                }
            }
            else if (SettingsWindow.Active)
            {
                // Fix all tooltip positions
                foreach (Window tooltip in Tooltips)
                {
                    tooltip.Bounds.X = Input.Mouse.X + 24;
                    tooltip.Bounds.Y = Input.Mouse.Y + 16;
                }

                // Update the window
                SettingsWindow.Update(gameTime);

                // Set the relative position
                relative = new Vector2(SettingsWindow.Bounds.X, SettingsWindow.Bounds.Y);

                #region Settings Component Checks
                // Full screen setting
                Checkbox checkbox = (Checkbox)SettingsWindow.GetComponent("fullscreen");
                Database.settings.Fullscreen = checkbox.Checked;

                // OS Cursor setting
                checkbox = (Checkbox)SettingsWindow.GetComponent("cursor");
                Database.settings.OSCursor = checkbox.Checked;
                if (checkbox.MouseOver(relative))
                    Tooltips[0].Active = true;
                else
                    Tooltips[0].Active = false;

                // Gamepad setting
                checkbox = (Checkbox)SettingsWindow.GetComponent("gamepad");
                Database.settings.Gamepad = checkbox.Checked;
                if (checkbox.MouseOver(relative))
                    Tooltips[1].Active = true;
                else
                    Tooltips[1].Active = false;

                // Health plates setting
                checkbox = (Checkbox)SettingsWindow.GetComponent("hints");
                Database.settings.Hints = checkbox.Checked;
                if (checkbox.MouseOver(relative))
                    Tooltips[2].Active = true;
                else
                    Tooltips[2].Active = false;

                // Health glow setting
                checkbox = (Checkbox)SettingsWindow.GetComponent("glow");
                Database.settings.ScreenGlow = checkbox.Checked;
                if (checkbox.MouseOver(relative))
                    Tooltips[3].Active = true;
                else
                    Tooltips[3].Active = false;

                // Keyboard configuration button clicks
                for (int i = 0; i < BindKeys.Length; i++)
                {
                    b = (Button)SettingsWindow.GetComponent(BindKeys[i]);
                    if (b.Clicked)
                    {
                        Audio.PlaySound("decision.ogg");
                        BindingIndex = i;
                        BindWindow.Active = true;
                    }
                }

                // Audio volume buttons
                b = (Button)SettingsWindow.GetComponent("svolume+");
                if (b.Clicked)
                {
                    Audio.PlaySound("decision.ogg");
                    if(Database.settings.SoundVolume < 1f)
                        Database.settings.SoundVolume += 0.05f;
                    RedrawSettings();
                }
                b = (Button)SettingsWindow.GetComponent("svolume-");
                if (b.Clicked)
                {
                    Audio.PlaySound("decision.ogg");
                    if (Database.settings.SoundVolume > 0f)
                        Database.settings.SoundVolume -= 0.05f;
                    RedrawSettings();
                }
                b = (Button)SettingsWindow.GetComponent("avolume+");
                if (b.Clicked)
                {
                    Audio.PlaySound("decision.ogg");
                    if (Database.settings.AmbienceVolume < 1f)
                        Database.settings.AmbienceVolume += 0.05f;
                    RedrawSettings();
                }
                b = (Button)SettingsWindow.GetComponent("avolume-");
                if (b.Clicked)
                {
                    Audio.PlaySound("decision.ogg");
                    if (Database.settings.AmbienceVolume > 0f)
                        Database.settings.AmbienceVolume -= 0.05f;
                    RedrawSettings();
                }
                b = (Button)SettingsWindow.GetComponent("mvolume+");
                if (b.Clicked)
                {
                    Audio.PlaySound("decision.ogg");
                    if (Database.settings.MusicVolume < 1f)
                        Database.settings.MusicVolume += 0.05f;
                    RedrawSettings();
                }
                b = (Button)SettingsWindow.GetComponent("mvolume-");
                if (b.Clicked)
                {
                    Audio.PlaySound("decision.ogg");
                    if (Database.settings.MusicVolume > 0f)
                        Database.settings.MusicVolume -= 0.05f;
                    RedrawSettings();
                }

                // Defaults button
                b = (Button)SettingsWindow.GetComponent("default");
                if (b.Clicked)
                {
                    Audio.PlaySound("decision.ogg");
                    Database.settings = new Settings();
                    RedrawSettings();
                }

                // Save and close button
                b = (Button)SettingsWindow.GetComponent("close");
                if (b.Clicked)
                {
                    Audio.PlaySound("decision.ogg");
                    Database.settings.SaveSettings();
                    Game.scene.ToggleInterface("settings_window", false);
                }
                #endregion
            }
        }

        /// <summary>
        /// Redraws the entire settings window.
        /// </summary>
        private void RedrawSettings()
        {
            SettingsWindow.Clear();
            #region Add components to the settings window
            // Core checkboxes
            SettingsWindow.AddComponent("fullscreen", new Checkbox("Windowed Fullscreen",
                new Vector2(4, 32), Color.White, Database.settings.Fullscreen));
            SettingsWindow.AddComponent("cursor", new Checkbox("Use OS cursor",
                new Vector2(4, 52), Color.White, Database.settings.OSCursor));
            SettingsWindow.AddComponent("gamepad", new Checkbox("Use gamepad",
                new Vector2(4, 72), Color.White, Database.settings.Gamepad));
            SettingsWindow.AddComponent("hints", new Checkbox("Enable hints",
                new Vector2(4, 92), Color.White, Database.settings.Hints));
            SettingsWindow.AddComponent("glow", new Checkbox("Screen glow",
                new Vector2(4, 112), Color.White, Database.settings.ScreenGlow));

            // Sound volume buttons
            SettingsWindow.AddComponent("svolume+", new Button("+", new Vector2(562, 32), Color.White));
            SettingsWindow.AddComponent("svolume-", new Button("-", new Vector2(600, 32), Color.White));
            SettingsWindow.AddComponent("avolume+", new Button("+", new Vector2(562, 64), Color.White));
            SettingsWindow.AddComponent("avolume-", new Button("-", new Vector2(600, 64), Color.White));
            SettingsWindow.AddComponent("mvolume+", new Button("+", new Vector2(562, 96), Color.White));
            SettingsWindow.AddComponent("mvolume-", new Button("-", new Vector2(600, 96), Color.White));

            // Draw volume text
            int sound = (int)(100f * Database.settings.SoundVolume),
                ambience = (int)(100f * Database.settings.AmbienceVolume),
                music = (int)(100f * Database.settings.MusicVolume);
            SettingsWindow.AddComponent("svolume", new Label("Sound Volume " + (Math.Round(sound / 5.0) * 5) + "%",
                new Vector2(380, 34), Color.White, false));
            SettingsWindow.AddComponent("avolume", new Label("Ambience Volume " + (Math.Round(ambience / 5.0) * 5) + "%",
                new Vector2(380, 66), Color.White, false));
            SettingsWindow.AddComponent("mvolume", new Label("Music Volume " + (Math.Round(music / 5.0) * 5) + "%",
                new Vector2(380, 98), Color.White, false));

            // Keyboard bindings
            SettingsWindow.AddComponent("keyboard", new Label("Keyboard Bindings", new Vector2(4, 144), Color.White, false));

            // Core keys
            SettingsWindow.AddComponent("upkey", new Button("Up : " + Database.settings.UpKey.KeyString, new Vector2(78, 174), Color.White, 240));
            SettingsWindow.AddComponent("leftkey", new Button("Left : " + Database.settings.LeftKey.KeyString, new Vector2(78, 198), Color.White, 240));
            SettingsWindow.AddComponent("downkey", new Button("Down : " + Database.settings.DownKey.KeyString, new Vector2(78, 222), Color.White, 240));
            SettingsWindow.AddComponent("rightkey", new Button("Right : " + Database.settings.RightKey.KeyString, new Vector2(78, 246), Color.White, 240));
            SettingsWindow.AddComponent("pulse", new Button("Power Pulse : " + Database.settings.PulseKey.KeyString, new Vector2(322, 174), Color.White, 240));
            SettingsWindow.AddComponent("sound", new Button("Sound Wave : " + Database.settings.SoundKey.KeyString, new Vector2(322, 198), Color.White, 240));
            SettingsWindow.AddComponent("interact", new Button("Interact : " + Database.settings.InteractKey.KeyString, new Vector2(322, 222), Color.White, 240));
            SettingsWindow.AddComponent("data", new Button("Data Access : " + Database.settings.DataKey.KeyString, new Vector2(322, 246), Color.White, 240));

            // Add the defaults button
            SettingsWindow.AddComponent("default", new Button("Defaults", new Vector2(196, 278), Color.White, 240));

            // Add the save button
            SettingsWindow.AddComponent("close", new Button("OK", new Vector2(196, 302), Color.White, 240));
            #endregion
        }

        /// <summary>
        /// Redraws the keys to the keyboard configuration buttons.
        /// Must be called after changing a keybinding.
        /// </summary>
        private void RedrawKeys()
        {
            // Set up the variables
            Button b; Keybinding bind;

            // Set the new values
            for (int i = 0; i < BindKeys.Length; i++)
            {
                // Gets the binding
                bind = Database.settings.GetBinding(i);

                // Get the button
                b = (Button)SettingsWindow.GetComponent(BindKeys[i]);

                // Set the new button text
                b.Text = b.Text.Substring(0, b.Text.LastIndexOf(':') + 1) + " " + bind.KeyString;
            }
        }

        /// <summary>
        /// Draws the settings component.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, Vector2 relative)
        {
            // Draw the settings window
            SettingsWindow.Draw(Game.spriteBatch);

            // Draw all tooltips
            foreach (Window tooltip in Tooltips)
                tooltip.Draw(Game.spriteBatch);

            // Draw binding window
            BindWindow.Draw(Game.spriteBatch);
        }
    }
}
