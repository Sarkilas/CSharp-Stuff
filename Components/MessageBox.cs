using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Data;
using Ingenia.Engine;

namespace Ingenia.Interface
{
    /// <summary>
    /// The message box class. Stores, handles and displays message box data.
    /// </summary>
    public class MessageBox
    {
        string Name;
        string Text;
        Texture2D Face;
        int Type;
        int Index;
        public bool Visible = false;
        float Fade = 0f;
        int WriteTime = 0;
        bool CanSkip = true;
        Window box = new Window("", new Rectangle(240, 400, 320, 160), new Color(160, 109, 86));
        SpriteFont Font = Game.Fonts["ChatFont"];

        /// <summary>
        /// Initializes the message box.
        /// Special text characters: / = 1 second pause, | = 0.5 second pause, % = disable text skip (this is a toggle)
        /// Slash commands (initialized with a backslash \):
        ///     \color (replace color with the color name in lowercase)
        /// </summary>
        public void Initialize()
        {
            // Set name
            if (Temp.Get("message_name") != null)
                Name = (string)Temp.Get("message_name");
            else
                Name = null;

            // Set face
            if (Temp.Get("message_face") != null)
                Face = GameData.GetTexture((string)Temp.Get("message_face"));
            else
                Face = null;

            // Set text
            Text = (string)Temp.Get("message_body");

            // Set type
            Type = (int)Temp.Get("message_type");

            // Set index to 0
            Index = 0;

            // Set title
            box.Title = Name == null ? "" : Name;

            // Set visible flag
            Visible = true; Fade = 0f;
        }

        /// <summary>
        /// Updates the message box. Required for continuing message writing.
        /// </summary>
        /// <param name="gameTime">A snapshot of game timings.</param>
        public void Update(GameTime gameTime)
        {
            // Ignore if hidden
            if (!Visible) return;

            // Update fade
            if (Fade < 1f)
            {
                Fade += .1f;
                return; 
            }

            // Return if index equals the length of the string
            if (Index >= Text.Length) return;

            // If skip key is pressed: skip
            if(CanSkip && (Input.LeftMouse ||  
                Input.Trigger(Keys.Enter) || Input.PadTrigger(Buttons.A)))
            {
                Index = Text.Length;
                return;
            }

            // Check for special commands at the index
            if (Text[Index] == '/')
            {
                WriteTime += 1000;
                Index++;
                return;
            }
            else if (Text[Index] == '|')
            {
                WriteTime += 500;
                Index++;
                return;
            }
            else if (Text[Index] == '%')
            {
                CanSkip = !CanSkip;
                Index++;
                return;
            }

            // Update write time
            if (WriteTime > 0)
                WriteTime -= gameTime.ElapsedGameTime.Milliseconds;
            else
            {
                Index++;
                WriteTime = 25;
            }
        }

        /// <summary>
        /// Draws the message box in its current state.
        /// </summary>
        public void Draw()
        {
            // Return if hidden
            if (!Visible) return;

            // Draw box first
            box.Draw(Game.spriteBatch, Fade);

            // Get actual drawing text
            string text = Text.Substring(0, Index);

            // Replace special characters
            text = text.Replace("/", String.Empty);
            text = text.Replace("|", String.Empty);
            text = text.Replace("%", String.Empty);

            // Get ready for word wrapping
            StringBuilder ob = new StringBuilder(text),
                target = new StringBuilder();

            // Word wrap
            Tools.WordWrap(ob, target, Font, new Rectangle(box.Bounds.X + 8, 
                box.Bounds.Y + 32, box.Bounds.Width - 16, box.Bounds.Height - 40), 1f);

            // Draw the text
            Tools.DrawShadowText(target.ToString(), new Vector2(box.Bounds.X + 8, 
                box.Bounds.Y + 32), Color.White, Color.Black, Font, Game.spriteBatch);
        }
    }
}
