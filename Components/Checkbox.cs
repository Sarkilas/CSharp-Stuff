using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ingenia.Engine;
using Ingenia.Data;

namespace Ingenia.Interface
{
    public class Checkbox : Component
    {
        // Static textures
        static Texture2D Box = GameData.GetTexture("Checkbox.png");
        static Texture2D Highlight = GameData.GetTexture("Checkbox_Highlight.png");
        static Texture2D CheckIcon = GameData.GetTexture("Checkbox_Icon.png");

        // Fields
        public string Text;
        public Color color;
        public Vector2 Position;
        public bool Checked = false;
        bool Clicked = false;

        public override bool Boolean
        {
            get
            {
                return Checked;
            }
        }

        // Constructor
        public Checkbox(string text, Vector2 position, Color color, bool check = false)
        {
            Text = text;
            Position = position;
            this.color = color;
            Checked = check;
            Vector2 c = Font.MeasureString(text);
            Bounds = new Rectangle((int)position.X, (int)position.Y,
                (int)c.X + 20, 20);
        }

        // Update
        public override void Update(GameTime gameTime, Vector2 relative)
        {
            // Set bounds
            Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X, 
                (int)relative.Y + Bounds.Y, Bounds.Width, Bounds.Height);
            // If mouse over
            if (Interpreter.EventRun) return;
            if (Input.MouseOver(newBounds))
            {
                if (Clicked && Input.Mouse.LeftButton == ButtonState.Released){
                    Checked = !Checked;
                    Audio.PlaySound("decision.ogg");
                }
                Clicked = Input.Mouse.LeftButton == ButtonState.Pressed;
            }
            else
                Clicked = false;
        }

        public bool MouseOver(Vector2 relative)
        {
            // Set bounds
            Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X,
                (int)relative.Y + Bounds.Y, Bounds.Width, Bounds.Height);

            // Return the bool
            return Input.MouseOver(newBounds);
        }

        // Draw
        public override void Draw(SpriteBatch spriteBatch, Vector2 relative)
        {
            // Set bounds
            Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X,
                (int)relative.Y + Bounds.Y, Bounds.Width, Bounds.Height);
            
            // Draw box
            spriteBatch.Draw(Box, new Vector2(newBounds.X, newBounds.Y + 3), color);

            // If checked: draw check icon
            if (Checked)
                spriteBatch.Draw(CheckIcon, new Vector2(newBounds.X, newBounds.Y + 3), color * 2f);

            // If mouse over: draw highlight
            if (Input.MouseOver(newBounds))
                spriteBatch.Draw(Highlight, new Vector2(newBounds.X, newBounds.Y + 3), color);

            // Draw text
            if (Input.MouseOver(newBounds))
                Tools.DrawShadowText(Text, new Vector2(newBounds.X + 20, newBounds.Y + 2), 
                    Color.White, Color.Black, Font, spriteBatch);
            else
                Tools.DrawShadowText(Text, new Vector2(newBounds.X + 20, newBounds.Y + 2),
                    color * 1.5f, Color.Black, Font, spriteBatch);
        }
    }
}
