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
    /// The drop down component class.
    /// </summary>
    class DropDown : Component
    {
        /// <summary>
        /// The textures required for this component.
        /// </summary>
        Texture2D pixel = GameData.GetTexture("white.png"),
            background = GameData.GetTexture("button.png"),
            dropdown = GameData.GetTexture("drop_down.png"),
            dropup = GameData.GetTexture("drop_up.png");

        /// <summary>
        /// The items in this drop down component.
        /// </summary>
        List<string> Items { get; set; }

        /// <summary>
        /// The selected item index.
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// The selected item.
        /// </summary>
        public string SelectedItem { 
            get {
                if (Items.Count == 0) return "";
                if (SelectedIndex >= Items.Count)
                    SelectedIndex = Items.Count - 1;
                else if (SelectedIndex < 0)
                    SelectedIndex = 0;
                return Items[SelectedIndex];
            }
        }

        /// <summary>
        /// True if the drop down list is open.
        /// </summary>
        public bool Open { get; set; }

        /// <summary>
        /// The width of this drop down.
        /// </summary>
        public int Width { get; set; }

        // Set flags
        bool Click = false;
        bool Hover = false,
            Updating = false;

        public bool Highlight = false;

        // Clicked property
        public bool Clicked
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
        /// Dynamic list.
        /// </summary>
        public string Dynamic = null;

        /// <summary>
        /// List of data elements.
        /// </summary>
        public List<DataElement> Elements { get; set; }

        /// <summary>
        /// Constructs a new drop down component.
        /// </summary>
        public DropDown(int x, int y, int width, string dynamic, List<DataElement> elements)
        {
            // Set base properties
            Items = new List<string>();

            // Set elements
            Elements = elements;
            
            // Set base properties
            SelectedIndex = 0; 
            Open = false;
            Width = width;
            Dynamic = dynamic;

            // Create bounds
            Bounds = new Rectangle(x, y, width - 16, 32);
        }

        // Update
        public override void Update(GameTime gameTime, Vector2 relative)
        {
            if (!Interpreter.LockControls)
            {
                Interpreter.EventRun = Open;
                Interpreter.LockControls = Open;
            }
            if (Elements.Count > 0)
            {
                List<string> items = new List<string>();
                // Load items
                if (Dynamic != null)
                {
                    if (Dynamic.StartsWith("[data]") && Dynamic.Length > 6)
                    {
                        Base list = GameData.GetData(Dynamic.Substring(6));
                        foreach (KeyValuePair<string, DataProperty> pair in list.Properties)
                            if (pair.Key != "key")
                                items.Add(pair.Value.String);
                    }
                }
                foreach (DataElement ims in Elements)
                    if (ims.Name.ToLower() == "items")
                    {
                        if (!Interpreter.EventConditionMet(ims, this)) continue;
                        foreach (KeyValuePair<string, DataProperty> pair in ims.Properties)
                            items.Add(pair.Value.String);
                    }

                if (items.Count != Items.Count && items.Count > 0)
                    Items = items;
            }
            if (SelectedIndex >= Items.Count) SelectedIndex = Items.Count - 1;
            if (SelectedIndex < 0) SelectedIndex = 0;
            // Set bounds
            Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X,
                (int)relative.Y + Bounds.Y, Bounds.Width + 16, Bounds.Height);
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
            // Click event (core)
            Rectangle newBounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width + 16, Bounds.Height);
            if (Clicked && Input.MouseOver(newBounds))
            {
                Audio.PlaySound("decision.ogg");
                Open = !Open;
            }

            // Go through each event
            foreach (DataElement element in Events)
            {
                // Ignore if property is missing
                if (!element.Properties.ContainsKey("name"))
                    continue;

                // Tick event
                if (element.Properties["name"].Value.ToLower() == "tick")
                    Interpreter.ExecuteEvent(this, element);
            }
        }

        // Draw button background
        private void DrawButton(SpriteBatch spriteBatch, Rectangle bounds, Color color, Texture2D replace = null)
        {
            // Draws the button background
            spriteBatch.Draw(replace == null ? pixel : replace, new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height), color);
        }

        // Draw
        public override void Draw(SpriteBatch spriteBatch, Vector2 relative)
        {
            if (SelectedIndex >= Items.Count) SelectedIndex = Items.Count - 1;
            if (SelectedIndex < 0) SelectedIndex = 0;
            // Set bounds
            Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X,
                (int)relative.Y + Bounds.Y, Bounds.Width, Bounds.Height);
            Rectangle hoverBounds = new Rectangle(newBounds.X, newBounds.Y,
                newBounds.Width + 16, newBounds.Height);
            // If clicked
            Color thisColor = Color.Lerp(Color.Gray, Color.White, .35f);
            if (Input.MouseOver(hoverBounds) && Enabled && Updating)
            {
                thisColor = Color.White;
                if (!Hover) Audio.PlaySound("cursor.ogg");
                Hover = true;
                DrawButton(spriteBatch, newBounds, thisColor * .75f, background);
            }
            else
            {
                if (Enabled)
                    DrawButton(spriteBatch, newBounds, thisColor * .75f, background);
                else DrawButton(spriteBatch, newBounds, Color.Lerp(thisColor, Color.Black, .8f) * .75f, background);
                thisColor = Enabled ? thisColor : Color.Lerp(thisColor, Color.Black, .5f);
                Hover = false;
            }

            Color textColor = thisColor;
            thisColor = Color.Lerp(new Color(110, 69, 46), Color.Black, .25f);

            // Draw borders
            Rectangle destination = new Rectangle(newBounds.X, newBounds.Y, newBounds.Width, 1);
            spriteBatch.Draw(pixel, destination, thisColor * .75f);
            destination = new Rectangle(newBounds.X, newBounds.Y, 1, newBounds.Height);
            spriteBatch.Draw(pixel, destination, thisColor * .75f);
            destination = new Rectangle(newBounds.X + Bounds.Width, newBounds.Y, 1, newBounds.Height);
            spriteBatch.Draw(pixel, destination, thisColor * .75f);
            destination = new Rectangle(newBounds.X, newBounds.Y + newBounds.Height, newBounds.Width, 1);
            spriteBatch.Draw(pixel, destination, thisColor * .75f);

            // Draw text
            Vector2 size = Font.MeasureString(SelectedItem),
                position = new Vector2(newBounds.X + 4, newBounds.Y + 16 - size.Y / 2);
            if (Input.MouseOver(newBounds) && Enabled && Updating)
                Tools.DrawShadowText(SelectedItem, position, textColor, Color.Black * .75f, Font, spriteBatch);
            else
                Tools.DrawShadowText(SelectedItem, position,
                    textColor, Color.Black * .75f, Font, spriteBatch);

            // Draw the drop down icon
            Texture2D texture = Open ? dropup : dropdown;
            Color color = Input.MouseOver(hoverBounds) ? Color.White : Color.Gray;
            spriteBatch.Draw(texture, new Vector2(newBounds.X + newBounds.Width, newBounds.Y), color);

            // Draw open menu if open
            if (Open)
            {
                SpriteFont font = Game.Fonts["DamageFont"];
                size = font.MeasureString("W");

                Rectangle bounds = new Rectangle(newBounds.X, newBounds.Y + newBounds.Height + 1,
                    newBounds.Width + 16, (int)size.Y * Items.Count);
                thisColor = new Color(60, 29, 16);

                DrawButton(spriteBatch, bounds, thisColor);

                // Draw borders
                thisColor = Color.Lerp(thisColor, Color.Black, .4f);
                destination = new Rectangle(bounds.X, bounds.Y, bounds.Width, 1);
                spriteBatch.Draw(pixel, destination, thisColor * .75f);
                destination = new Rectangle(bounds.X, bounds.Y, 1, bounds.Height);
                spriteBatch.Draw(pixel, destination, thisColor * .75f);
                destination = new Rectangle(bounds.X + bounds.Width, bounds.Y, 1, bounds.Height);
                spriteBatch.Draw(pixel, destination, thisColor * .75f);
                destination = new Rectangle(bounds.X, bounds.Y + bounds.Height, bounds.Width, 1);
                spriteBatch.Draw(pixel, destination, thisColor * .75f);

                // Draw every item
                for (int i = 0; i < Items.Count; i++)
                {
                    Rectangle item = new Rectangle(bounds.X, bounds.Y +
                        (int)size.Y * i, bounds.Width, (int)size.Y);
                    if (Input.MouseOver(item))
                    {
                        DrawButton(spriteBatch, item, new Color(200, 109, 66));
                        if (Input.MouseOver(item) && Input.LeftMouse && Enabled)
                        {
                            Audio.PlaySound("decision.ogg");
                            SelectedIndex = i;
                            Open = false;
                        }
                    }
                    else if (SelectedIndex == i)
                        DrawButton(spriteBatch, item, new Color(110, 69, 46));

                    Tools.DrawShadowText(Items[i], new Vector2(bounds.X + 2, bounds.Y + (size.Y) * i),
                        Color.White, Color.Black * .5f, font, spriteBatch);
                }
            }

            // Disable updating
            Updating = false;
        }
    }
}
