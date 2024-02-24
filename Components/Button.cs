using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Engine;
using Ingenia.Data;

namespace Ingenia.Interface
{
    public class Button : Component
    {
        // Textures
        static Texture2D ButtonImage = GameData.GetTexture("Button.png"),
            Border = GameData.GetTexture("Border.png");

        // Fields
        public string Text;
        public Vector2 Position;
        public Color color, hoverColor, clickColor;

        // Set flags
        bool Click = false;
        bool Hover = false,
            Updating = false;

        bool NoCenter { get; set; }

        public bool Highlight = false;

        // Clicked property
        public override bool Clicked
        {
            get
            {
                if (Click)
                {
                    Click = false;
                    return true;
                }
                return false;
            }
        }

        // Constructor
        public Button(string text, Vector2 position, Color color, int width = 0, bool adjustRight = false, bool nocenter = false)
        {
            Text = text;
            Position = position;
            Vector2 check = Font.MeasureString(Text);
            Bounds = new Rectangle((int)position.X, (int)position.Y,
                (int)check.X + 24, (int)check.Y + 4);
            if (width > 0) _bounds.Width = width;
            if(adjustRight)
                _bounds.X -= (int)check.X + 24;
            this.color = color;
            hoverColor = Color.Orange;
            clickColor = Color.LightGreen;
            Font = Game.Fonts["ChatFont"];
            NoCenter = nocenter;
        }

        // Update
        public override void Update(GameTime gameTime, Vector2 relative)
        {
            // Set bounds
            Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X,
                (int)relative.Y + Bounds.Y, Bounds.Width, Bounds.Height);

            // If clicked
            if (Input.MouseOver(newBounds) && Input.LeftMouse && Enabled)
                Click = true;

            // Update events
            UpdateEvents();

            // Set updating flag
            Updating = true;

            // Return if no events
            if (Events.Count == 0) return;

            // If no events can run: set updating to false
            foreach (DataElement element in Events)
            {
                if (element.Properties.ContainsKey("name"))
                    if (element.Properties["name"].Value.ToLower() == "tick")
                        continue;
                if (Interpreter.EventConditionMet(element, this))
                    return;
            }

            // Set false
            Updating = false;
        }

        /// <summary>
        /// Updates the events for this component.
        /// </summary>
        private void UpdateEvents()
        {
            // Go through each event
            foreach (DataElement element in Events)
            {
                // Ignore if property is missing
                if (!element.Properties.ContainsKey("name"))
                    continue;

                // Click event
                if (element.Properties["name"].Value.ToLower() == "clicked" && Clicked && Enabled)
                    Interpreter.ExecuteEvent(this, element);

                // Tick event
                if (element.Properties["name"].Value.ToLower() == "tick")
                    Interpreter.ExecuteEvent(this, element);
            }
        }

        // Draw button background
        private void DrawButton(SpriteBatch spriteBatch, Rectangle bounds, Color color)
        {
            // Draws the button background
            spriteBatch.Draw(ButtonImage, new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height), color);
        }

        // Draw
        public override void Draw(SpriteBatch spriteBatch, Vector2 relative)
        {
            // Set bounds
            Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X,
                (int)relative.Y + Bounds.Y, Bounds.Width, Bounds.Height);
            // If clicked
            Color thisColor;
            if (Click && Enabled && Updating)
            {
                thisColor = clickColor;
                DrawButton(spriteBatch, newBounds, clickColor * .75f);
            }
            else if (Input.MouseOver(newBounds) && Enabled && Updating)
            {
                thisColor = hoverColor;
                if (!Hover) Audio.PlaySound("cursor.ogg");
                Hover = true;
                DrawButton(spriteBatch, newBounds, hoverColor * .75f);
            }
            else {
                if(Enabled)
                    DrawButton(spriteBatch, newBounds, Color.Lerp(color, Color.Black, Highlight ? .1f : .5f) * .9f);
                else DrawButton(spriteBatch, newBounds, Color.Lerp(color, Color.Black, .8f) * .75f);
                thisColor = Enabled ? color : Color.Lerp(color, Color.Black, .5f);
                Hover = false; 
            }

            Color textColor = thisColor;

            // Draw borders
            Rectangle destination = new Rectangle(newBounds.X, newBounds.Y, newBounds.Width, 1);
            spriteBatch.Draw(Border, destination, thisColor * .75f);
            destination = new Rectangle(newBounds.X, newBounds.Y, 1, newBounds.Height);
            spriteBatch.Draw(Border, destination, thisColor * .75f);
            destination = new Rectangle(newBounds.X + Bounds.Width, newBounds.Y, 1, newBounds.Height);
            spriteBatch.Draw(Border, destination, thisColor * .75f);
            destination = new Rectangle(newBounds.X, newBounds.Y + newBounds.Height, newBounds.Width, 1);
            spriteBatch.Draw(Border, destination, thisColor * .75f);

            // Draw text
            Vector2 size = Font.MeasureString(Text),
                position = NoCenter ? new Vector2(newBounds.X + 4, newBounds.Y + 3) :
                    new Vector2(newBounds.X + newBounds.Width / 2 - (int)size.X / 2,
                        newBounds.Y + newBounds.Height / 2 - (int)size.Y / 2);
            if(Input.MouseOver(newBounds) && Enabled && Updating)
                Tools.DrawShadowText(Text, position, hoverColor, Color.Black * .75f, Font, spriteBatch);
            else
                Tools.DrawShadowText(Text, position,
                    textColor, Color.Black * .75f, Font, spriteBatch);

            // Disable updating
            Updating = false;
        }
    }
}
