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
    public class Label : Component
    {
        // Text variable
        public string Text;
        Color color;
        Color hoverColor;
        public bool Clickable;
        SpriteFont thisFont;

        // Set flags
        bool Click = false;
        bool Hover = false;
        bool Updating = false;

        // Center flag
        public bool Center { get; set; }
        public bool AdjustRight { get; set; }
        private Vector2 Position;

        // Constructor
        public Label(string text, Vector2 position, Color color, bool center, bool clickable = false, SpriteFont font = null, bool adjust = false)
        {
            Text = text;
            this.color = color;
            hoverColor = new Color(255, 160, 90);
            Clickable = clickable;
            thisFont = font == null ? Font : font;
            Center = center;
            AdjustRight = adjust;
            Position = position; 
            Refresh();
        }

        private void Refresh()
        {
            string temp = Text;
            if (temp.EndsWith("}") && Script.Run(temp) != null)
                temp = (string)Script.Run(temp);
            Vector2 size = thisFont.MeasureString(temp);
            Bounds = new Rectangle((int)Position.X, (int)Position.Y,
                (int)size.X, (int)size.Y);
        }

        // Set font
        public SpriteFont FontFile
        {
            set
            {
                thisFont = value;
                Vector2 size = thisFont.MeasureString(Text);
                Bounds = new Rectangle(Bounds.X, Bounds.Y,
                    (int)size.X, (int)size.Y);
            }
        }

        // Set hover color
        public Color HoverColor
        {
            set
            {
                hoverColor = value;
            }
        }

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

        // Update inherit
        public override void Update(GameTime gameTime, Vector2 relative)
        {
            // Refresh txt and contents when needed
            Refresh();

            // Set bounds
            Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X,
                (int)relative.Y + Bounds.Y, Bounds.Width, Bounds.Height);

            // Fix bounds if centered
            if (AdjustRight)
            {
                newBounds.X -= newBounds.Width;
                if (Center)
                    newBounds.Y -= newBounds.Height / 2;
            }
            else if (Center)
            {
                newBounds.X -= newBounds.Width / 2;
                newBounds.Y -= newBounds.Height / 2;
            }
            Rectangle overBounds = new Rectangle(newBounds.X, newBounds.Y + 8,
                newBounds.Width, newBounds.Height - 12);
            if (newBounds.Height <= 24)
                overBounds = newBounds;
            if (Clickable && Input.MouseOver(overBounds) && Enabled)
            {
                if (!Hover && !Inactive)
                {
                    Audio.PlaySound("cursor.ogg");
                    Hover = true;
                }
                if (Input.LeftMouse && !Inactive)
                    Click = true;
                Component.MouseOver = true;
            }
            else { Hover = false; }

            // Update events
            UpdateEvents();

            // Set updating flag
            Updating = true;
        }

        private bool Inactive
        {
            get
            {
                // If no events can run: set updating to false
                foreach (DataElement element in Events)
                    if (Interpreter.EventConditionMet(element, this))
                        return false;
                return (Events.Count > 0);
            }
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
                if (element.Properties["name"].Value.ToLower() == "clicked" && Click)
                    Interpreter.ExecuteEvent(this, element);

                // Tick event
                if (element.Properties["name"].Value.ToLower() == "tick")
                    Interpreter.ExecuteEvent(this, element);
            }

            // Set click to false
            Click = false;
        }

        // Draw
        public override void Draw(SpriteBatch spriteBatch, Vector2 relative)
        {
            // Set bounds
            Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X,
                (int)relative.Y + Bounds.Y, Bounds.Width, Bounds.Height);
            // Fix bounds if centered
            if (AdjustRight)
            {
                newBounds.X -= newBounds.Width;
                if (Center)
                    newBounds.Y -= newBounds.Height / 2;
            }
            else if (Center)
            {
                newBounds.X -= newBounds.Width / 2;
                newBounds.Y -= newBounds.Height / 2;
            }
            string temp = Text;
            if (temp.EndsWith("}"))
                temp = (string)Script.Run(temp);
            Rectangle overBounds = new Rectangle(newBounds.X, newBounds.Y + 8, 
                newBounds.Width, newBounds.Height - 12);
            if (newBounds.Height <= 24)
                overBounds = newBounds;
            // Draw label
            if (Clickable && Input.MouseOver(overBounds) && Enabled && Updating && !Inactive)
                if (newBounds == overBounds)
                    Tools.DrawShadowText(temp,
                        new Vector2(newBounds.X, newBounds.Y), hoverColor, Color.Black * .5f, thisFont, spriteBatch);
                else
                    Tools.DrawShadowText(temp,
                        new Vector2(newBounds.X - 2, newBounds.Y - 2), hoverColor, Color.Black * .5f, thisFont, spriteBatch, 3);
            else
            {
                Color thisColor = color;
                if (!Enabled)
                    thisColor = Color.Lerp(thisColor, Color.DimGray, .8f);
                Tools.DrawShadowText(temp,
                    new Vector2(newBounds.X, newBounds.Y), thisColor, Color.Black * .5f, thisFont, spriteBatch);
            }
            Updating = false;
        }
    }
}
