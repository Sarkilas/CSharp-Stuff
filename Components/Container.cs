using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Data;
using Ingenia.Engine;

namespace Ingenia.Interface
{
    /// <summary>
    /// The container component class.
    /// </summary>
    class Container : Component
    {
        /// <summary>
        /// The textures for this component.
        /// </summary>
        static Texture2D Background = GameData.GetTexture("button.png"),
            Border = GameData.GetTexture("border.png");

        /// <summary>
        /// The static border color.
        /// </summary>
        private Color BorderColor
        {
            get
            {
                return new Color(110, 69, 46);
            }
        }

        /// <summary>
        /// The clicked flag.
        /// </summary>
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

        /// <summary>
        /// The right clicked flag.
        /// </summary>
        public override bool RightClicked
        {
            get
            {
                if (RClick)
                {
                    RClick = false;
                    return true;
                }
                return false;
            }
        }

        // Set flags
        bool Click = false, RClick = false;
        bool Hover = false,
            Updating = false;
        public bool Highlight = false;

        /// <summary>
        /// Constructs a new container component.
        /// </summary>
        public Container(int x, int y, int width, int height, object tag = null)
        {
            // Set up bounds
            Bounds = new Rectangle(x, y, width, height);

            // Set tag
            Tag = tag;
        }

        /// <summary>
        /// Updates the events for this component.
        /// </summary>
        private void UpdateEvents()
        {
            // Go through each event
            bool uniqueCheck = false;
            bool click = Clicked, rclick = RightClicked;
            foreach (DataElement element in Events)
            {
                // Ignore if property is missing
                if (!element.Properties.ContainsKey("name"))
                    continue;

                // If unique check is true and this event is unique: don't execute
                if(element.Properties.ContainsKey("unique"))
                    if (uniqueCheck && element.Properties["unique"].Boolean)
                        continue;

                // Click event
                if (element.Properties["name"].Value.ToLower() == "clicked" && click)
                {
                    // If successful: set unique check to true
                    if (Interpreter.EventConditionMet(element, this))
                        uniqueCheck = true;
                    Interpreter.ExecuteEvent(this, element);
                }

                // Right click event
                if (element.Properties["name"].Value.ToLower() == "right clicked" && rclick)
                {
                    // If successful: set unique check to true
                    if (Interpreter.EventConditionMet(element, this))
                        uniqueCheck = true;
                    Interpreter.ExecuteEvent(this, element);
                }

                // Mouse over event
                if (element.Properties["name"].Value.ToLower() == "mouse over" && Hover)
                {
                    // If successful: set unique check to true
                    if (Interpreter.EventConditionMet(element, this))
                        uniqueCheck = true;
                    Interpreter.ExecuteEvent(this, element);
                }
            }
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
            if (Input.MouseOver(newBounds) && Input.RightMouse && Enabled)
                RClick = true;
            // Update events
            if (Enabled)
                UpdateEvents();

            // Set updating flag
            Updating = true;

            // Return if no events
            if (Events.Count == 0) return;

            // If no events can run: set updating to false
            foreach (DataElement element in Events)
                if (Interpreter.EventConditionMet(element, this))
                    return;

            // Set false
            Updating = false;
        }

        // Draw background
        private void DrawBackground(SpriteBatch spriteBatch, Rectangle bounds, Color color)
        {
            // Draws the button background
            spriteBatch.Draw(Background, new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height), color);
        }

        // Draw
        public override void Draw(SpriteBatch spriteBatch, Vector2 relative)
        {
            // Set bounds
            Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X,
                (int)relative.Y + Bounds.Y, Bounds.Width, Bounds.Height);
            // If clicked
            if (Click && Enabled && Updating)
                DrawBackground(spriteBatch, newBounds, Color.Gray);
            else if (Input.MouseOver(newBounds) && Enabled && Updating)
            {
                if (!Hover) Audio.PlaySound("cursor.ogg");
                Hover = true;
                DrawBackground(spriteBatch, newBounds, Color.White);
            }
            else
            {
                if (Enabled)
                    DrawBackground(spriteBatch, newBounds, Highlight ? Color.LightGray : Color.Gray);
                else DrawBackground(spriteBatch, newBounds, Color.DimGray);
                Hover = false;
            }

            // Draw borders
            Rectangle destination = new Rectangle(newBounds.X, newBounds.Y, newBounds.Width, 1);
            spriteBatch.Draw(Border, destination, BorderColor * .75f);
            destination = new Rectangle(newBounds.X, newBounds.Y, 1, newBounds.Height);
            spriteBatch.Draw(Border, destination, BorderColor * .75f);
            destination = new Rectangle(newBounds.X + Bounds.Width, newBounds.Y, 1, newBounds.Height);
            spriteBatch.Draw(Border, destination, BorderColor * .75f);
            destination = new Rectangle(newBounds.X, newBounds.Y + newBounds.Height, newBounds.Width, 1);
            spriteBatch.Draw(Border, destination, BorderColor * .75f);

            // Disable updating
            Updating = false;
        }
    }
}
